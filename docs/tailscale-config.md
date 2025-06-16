# Tailscale Configuration

This guide provides detailed information about configuring and using Project ECHO with Tailscale.

## Overview

Project ECHO uses Tailscale for secure, encrypted communication between nodes. This ensures that all connections are private and authenticated, even across different networks.

## Network Architecture

```
[Client Browser] → [Tailscale Network] → [Project ECHO Server]
                                    ↓
[Database Servers] ← [Tailscale Network] → [Remote Systems]
```

## Port Configuration

The following ports need to be accessible over Tailscale:

- 8080: HTTP traffic
- 8443: HTTPS traffic
- 3306: MySQL (if using)
- 5432: PostgreSQL (if using)
- 1433: SQL Server (if using)

## Setting Up Tailscale

### 1. Install Tailscale

#### Windows
1. Download the installer from [Tailscale website](https://tailscale.com/download)
2. Run the installer
3. Sign in with your Tailscale account

#### Linux
```bash
# Add Tailscale repository
curl -fsSL https://pkgs.tailscale.com/stable/ubuntu/jammy.gpg | sudo tee /usr/share/keyrings/tailscale-archive-keyring.gpg >/dev/null
curl -fsSL https://pkgs.tailscale.com/stable/ubuntu/jammy.list | sudo tee /etc/apt/sources.list.d/tailscale.list

# Install Tailscale
sudo apt-get update
sudo apt-get install tailscale
```

#### macOS
1. Install from the [Mac App Store](https://apps.apple.com/app/tailscale/id1475387142)
2. Launch Tailscale
3. Sign in with your account

### 2. Configure Tailscale

1. Start Tailscale:
   ```bash
   sudo tailscale up
   ```

2. Get your Tailscale IP:
   ```bash
   tailscale ip
   ```

3. Configure ACLs (Access Control Lists):
   ```json
   {
     "acls": [
       {
         "action": "accept",
         "src": ["tag:project-echo"],
         "dst": ["tag:project-echo:*"]
       }
     ]
   }
   ```

### 3. Firewall Configuration

Allow the following ports in your firewall:

```bash
# Allow Tailscale
sudo ufw allow 41641/udp
sudo ufw allow 41641/tcp

# Allow Project ECHO ports
sudo ufw allow 8080/tcp
sudo ufw allow 8443/tcp
```

## Connecting to Services

### Database Connections

When connecting to databases over Tailscale:

1. Use the Tailscale IP instead of localhost
2. Configure database to listen on Tailscale interface
3. Update connection strings to use Tailscale IPs

Example connection string:
```
Server=<tailscale-ip>;Database=echodb;User Id=user;Password=password;
```

### Remote Access

For remote desktop and SSH access:

1. Ensure target machines are on Tailscale
2. Use Tailscale IPs for connections
3. Configure SSH to listen on Tailscale interface

## Security Considerations

1. **ACLs**: Use Tailscale ACLs to restrict access
2. **Tags**: Tag machines based on their role
3. **Exit Nodes**: Configure exit nodes for external access
4. **MagicDNS**: Use MagicDNS for easier hostname resolution

## Troubleshooting

### Common Issues

1. **Connection Problems**
   ```bash
   # Check Tailscale status
   tailscale status
   
   # Test connectivity
   tailscale ping <target-ip>
   ```

2. **Port Access**
   ```bash
   # Test port accessibility
   nc -zv <target-ip> <port>
   ```

3. **DNS Resolution**
   ```bash
   # Check MagicDNS
   nslookup <hostname>
   ```

### Debug Commands

```bash
# View Tailscale routes
tailscale netcheck

# Check ACLs
tailscale debug prefs

# View connected peers
tailscale status --peers
```

## Best Practices

1. **Network Organization**
   - Use consistent naming for machines
   - Tag machines based on their role
   - Document IP assignments

2. **Security**
   - Regularly update Tailscale
   - Review ACLs periodically
   - Monitor connection logs

3. **Performance**
   - Use direct connections when possible
   - Monitor network latency
   - Configure appropriate timeouts

## Support

For Tailscale-specific issues:
1. Check [Tailscale Documentation](https://tailscale.com/kb/)
2. Contact Tailscale Support
3. Visit [Tailscale Community](https://github.com/tailscale/tailscale/discussions) 