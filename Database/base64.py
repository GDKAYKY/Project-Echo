import base64
import os
from flask import Blueprint, request, jsonify, send_file
from werkzeug.utils import secure_filename
from io import BytesIO
import mimetypes

base64_bp = Blueprint('base64', __name__)

# Configurações
ALLOWED_EXTENSIONS = {'png', 'jpg', 'jpeg', 'gif', 'bmp', 'webp'}
MAX_FILE_SIZE = 10 * 1024 * 1024  # 10MB

def allowed_file(filename):
    return '.' in filename and \
           filename.rsplit('.', 1)[1].lower() in ALLOWED_EXTENSIONS

def format_bytes(size):
    """Formatar bytes em formato legível"""
    for unit in ['B', 'KB', 'MB', 'GB']:
        if size < 1024.0:
            return f"{size:.1f} {unit}"
        size /= 1024.0
    return f"{size:.1f} TB"

@base64_bp.route('/encode', methods=['POST'])
def encode_image():
    """Codifica uma imagem para Base64"""
    try:
        if 'file' not in request.files:
            return jsonify({'error': 'Nenhum arquivo foi enviado.'}), 400
        
        file = request.files['file']
        
        if file.filename == '':
            return jsonify({'error': 'Nenhum arquivo selecionado.'}), 400
        
        if not allowed_file(file.filename):
            return jsonify({'error': 'Tipo de arquivo não suportado. Use JPEG, PNG, GIF, BMP ou WebP.'}), 400
        
        # Verificar tamanho do arquivo
        file.seek(0, os.SEEK_END)
        file_size = file.tell()
        file.seek(0)
        
        if file_size > MAX_FILE_SIZE:
            return jsonify({'error': 'Arquivo muito grande. Tamanho máximo: 10MB.'}), 400
        
        # Ler e codificar arquivo
        file_data = file.read()
        base64_string = base64.b64encode(file_data).decode('utf-8')
        
        # Determinar tipo MIME
        mime_type = file.content_type or mimetypes.guess_type(file.filename)[0] or 'application/octet-stream'
        
        return jsonify({
            'base64': base64_string,
            'mimeType': mime_type,
            'fileName': secure_filename(file.filename),
            'size': file_size,
            'sizeFormatted': format_bytes(file_size),
            'dataUrl': f"data:{mime_type};base64,{base64_string}"
        })
        
    except Exception as e:
        return jsonify({'error': 'Erro interno do servidor.', 'details': str(e)}), 500

@base64_bp.route('/decode', methods=['POST'])
def decode_base64():
    """Decodifica Base64 para imagem"""
    try:
        data = request.get_json()
        
        if not data or 'base64Data' not in data:
            return jsonify({'error': 'Dados Base64 não fornecidos.'}), 400
        
        base64_data = data['base64Data']
        
        # Remover prefixo data URL se presente
        if ',' in base64_data:
            base64_data = base64_data.split(',')[1]
        
        # Decodificar Base64
        try:
            image_data = base64.b64decode(base64_data)
        except Exception:
            return jsonify({'error': 'Formato Base64 inválido.'}), 400
        
        # Determinar tipo de conteúdo e nome do arquivo
        mime_type = data.get('mimeType', 'image/png')
        filename = data.get('fileName', 'decoded_image.png')
        
        # Criar arquivo em memória
        img_io = BytesIO(image_data)
        img_io.seek(0)
        
        return send_file(
            img_io,
            mimetype=mime_type,
            as_attachment=True,
            download_name=secure_filename(filename)
        )
        
    except Exception as e:
        return jsonify({'error': 'Erro interno do servidor.', 'details': str(e)}), 500

@base64_bp.route('/validate', methods=['POST'])
def validate_base64():
    """Valida se uma string é Base64 válido"""
    try:
        data = request.get_json()
        
        if not data or 'base64Data' not in data:
            return jsonify({'isValid': False, 'error': 'Dados Base64 não fornecidos.'})
        
        base64_data = data['base64Data']
        
        # Remover prefixo data URL se presente
        if ',' in base64_data:
            base64_data = base64_data.split(',')[1]
        
        try:
            # Tentar decodificar
            decoded_data = base64.b64decode(base64_data)
            size = len(decoded_data)
            
            return jsonify({
                'isValid': True,
                'size': size,
                'sizeFormatted': format_bytes(size)
            })
            
        except Exception as e:
            return jsonify({'isValid': False, 'error': 'Formato Base64 inválido.'})
        
    except Exception as e:
        return jsonify({'isValid': False, 'error': str(e)})

