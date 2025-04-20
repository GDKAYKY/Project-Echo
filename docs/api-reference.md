# Project ECHO API Reference

This document details the RESTful API endpoints available in Project ECHO.

## API Overview

The Project ECHO API allows programmatic access to most functionality available in the web interface. The API uses standard HTTP methods and returns responses in JSON format.

### Base URL

```
https://your-server:5003/api/v1
```

### Authentication

All API requests require authentication using a Bearer token in the Authorization header:

```
Authorization: Bearer <your-api-token>
```

To obtain an API token, see the [Authentication](#authentication-api) section below.

### Response Format

All responses are in JSON format with the following structure:

```json
{
  "success": true|false,
  "data": {}, // Response data when success is true
  "error": {  // Error information when success is false
    "code": "ERROR_CODE",
    "message": "Human readable error message"
  }
}
```

## Authentication API

### Get API Token

```
POST /auth/token
```

Request body:
```json
{
  "username": "user@example.com",
  "password": "password"
}
```

Response:
```json
{
  "success": true,
  "data": {
    "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "expires": "2023-12-31T23:59:59Z"
  }
}
```

### Refresh Token

```
POST /auth/refresh
```

Request header:
```
Authorization: Bearer <your-current-token>
```

Response:
```json
{
  "success": true,
  "data": {
    "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "expires": "2023-12-31T23:59:59Z"
  }
}
```

## Database API

### List Database Connections

```
GET /databases
```

Response:
```json
{
  "success": true,
  "data": {
    "connections": [
      {
        "id": "conn-123",
        "name": "Production DB",
        "type": "mysql",
        "host": "db.example.com"
      },
      {
        "id": "conn-456",
        "name": "Test DB",
        "type": "postgres",
        "host": "test-db.example.com"
      }
    ]
  }
}
```

### Create Database Connection

```
POST /databases
```

Request body:
```json
{
  "name": "New Database",
  "type": "mysql",
  "host": "new-db.example.com",
  "port": 3306,
  "database": "mydb",
  "username": "user",
  "password": "password"
}
```

Response:
```json
{
  "success": true,
  "data": {
    "id": "conn-789",
    "name": "New Database",
    "type": "mysql",
    "host": "new-db.example.com"
  }
}
```

### Execute Query

```
POST /databases/{connectionId}/query
```

Request body:
```json
{
  "query": "SELECT * FROM users WHERE id = :userId",
  "parameters": {
    "userId": 123
  },
  "timeout": 30,
  "maxRows": 1000
}
```

Response:
```json
{
  "success": true,
  "data": {
    "columns": ["id", "name", "email", "created_at"],
    "rows": [
      [123, "John Doe", "john@example.com", "2023-01-15T12:00:00Z"]
    ],
    "rowCount": 1,
    "executionTime": 0.15
  }
}
```

## SSH API

### List SSH Connections

```
GET /ssh/connections
```

Response:
```json
{
  "success": true,
  "data": {
    "connections": [
      {
        "id": "ssh-123",
        "name": "Production Server",
        "host": "server.example.com",
        "username": "admin"
      }
    ]
  }
}
```

### Create SSH Session

```
POST /ssh/sessions
```

Request body:
```json
{
  "connectionId": "ssh-123",
  "terminalType": "xterm-256color",
  "terminalSize": {
    "cols": 80,
    "rows": 24
  }
}
```

Response:
```json
{
  "success": true,
  "data": {
    "sessionId": "sess-456",
    "webSocketUrl": "wss://your-server:5003/api/v1/ssh/sessions/sess-456/stream"
  }
}
```

## Remote Desktop API

### List Remote Desktop Connections

```
GET /remote/connections
```

Response:
```json
{
  "success": true,
  "data": {
    "connections": [
      {
        "id": "rdp-123",
        "name": "Windows Server",
        "host": "windows.example.com",
        "protocol": "rdp"
      }
    ]
  }
}
```

### Start Remote Session

```
POST /remote/sessions
```

Request body:
```json
{
  "connectionId": "rdp-123",
  "resolution": {
    "width": 1920,
    "height": 1080
  },
  "colorDepth": 24
}
```

Response:
```json
{
  "success": true,
  "data": {
    "sessionId": "rsess-789",
    "webSocketUrl": "wss://your-server:5003/api/v1/remote/sessions/rsess-789/stream"
  }
}
```

## Network API

### Get Network Statistics

```
GET /network/stats
```

Response:
```json
{
  "success": true,
  "data": {
    "connectedDevices": 15,
    "networkTraffic": {
      "inbound": 1024000,
      "outbound": 512000
    },
    "activeConnections": 32,
    "bandwidthUsage": 0.75
  }
}
```

### Run Network Tool

```
POST /network/tools/{toolName}
```

Example for ping tool:

Request body:
```json
{
  "target": "google.com",
  "count": 4
}
```

Response:
```json
{
  "success": true,
  "data": {
    "results": [
      {
        "sequence": 1,
        "time": 15.4,
        "ttl": 58
      },
      {
        "sequence": 2,
        "time": 14.2,
        "ttl": 58
      },
      {
        "sequence": 3,
        "time": 16.8,
        "ttl": 58
      },
      {
        "sequence": 4,
        "time": 15.1,
        "ttl": 58
      }
    ],
    "summary": {
      "transmitted": 4,
      "received": 4,
      "loss": 0,
      "minTime": 14.2,
      "avgTime": 15.38,
      "maxTime": 16.8
    }
  }
}
```

## Error Codes

| Code | Description |
|------|-------------|
| `AUTH_INVALID_CREDENTIALS` | Invalid username or password |
| `AUTH_TOKEN_EXPIRED` | The provided API token has expired |
| `AUTH_INSUFFICIENT_PERMISSIONS` | The user lacks permission for the requested operation |
| `DB_CONNECTION_FAILED` | Could not connect to the database |
| `DB_QUERY_ERROR` | Error executing the SQL query |
| `SSH_CONNECTION_FAILED` | Could not establish SSH connection |
| `SSH_AUTHENTICATION_FAILED` | SSH authentication failed |
| `RDP_CONNECTION_FAILED` | Could not establish remote desktop connection |
| `NETWORK_TOOL_FAILED` | Network tool execution failed |
| `VALIDATION_ERROR` | The request body failed validation |
| `RESOURCE_NOT_FOUND` | The requested resource was not found |
| `INTERNAL_ERROR` | An unexpected error occurred | 