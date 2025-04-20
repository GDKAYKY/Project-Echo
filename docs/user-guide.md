# Project ECHO User Guide

This guide provides detailed instructions on how to use Project ECHO's features.

## Navigation

The Project ECHO interface consists of a header at the top and a sidebar on the left. The sidebar contains icons for different features:

- **Database Search**: Run SQL queries against your databases
- **SSH Terminal**: Access remote systems via secure shell
- **Remote Access**: Connect to remote desktops
- **Documentation**: Access system documentation
- **Network**: Monitor network traffic and performance

Click on any icon to navigate to the corresponding feature.

## Database Search

The Database Search feature allows you to query databases using SQL.

### Setting Up Database Connections

1. Navigate to the Database Search page
2. Click on "Settings" to configure database connections
3. Add a new connection with the following information:
   - Connection Name
   - Database Type (MySQL, PostgreSQL, SQL Server, etc.)
   - Host/Server
   - Port
   - Database Name
   - Username
   - Password
4. Click "Save" to store the connection

### Running Queries

1. Select a database connection from the dropdown
2. Enter your SQL query in the query text area
3. Click "Run Query" to execute
4. Results will be displayed in a table below the query

## SSH Terminal

The SSH Terminal provides access to remote systems via command line.

### Setting Up SSH Connections

1. Navigate to the SSH Terminal page
2. Click "New Connection"
3. Enter the following details:
   - Connection Name
   - Host
   - Port (default: 22)
   - Username
   - Authentication Method (Password or Key)
   - Password or Key File
4. Click "Connect"

### Using the Terminal

- Type commands in the terminal window
- Use the terminal controls (red, yellow, green buttons) to close, minimize, or maximize
- Right-click for additional options (copy, paste, etc.)

## Remote Access

Remote Access allows you to connect to remote systems using graphical interfaces.

### Creating a Remote Connection

1. Navigate to the Remote Access page
2. Enter the following details:
   - Host/IP Address
   - Port
   - Username
   - Password
3. Click "Connect"

### Using Remote Desktop

- Use the mouse and keyboard as you would on a local machine
- Access additional options through the toolbar at the top of the remote window

## Documentation

The Documentation page provides access to system documentation and guides.

### Accessing Documentation

1. Navigate to the Documentation page
2. Browse the categories of documentation
3. Click on a document to view its contents

## Network

The Network page displays network statistics and provides tools for network analysis.

### Viewing Network Statistics

1. Navigate to the Network page
2. The dashboard displays various metrics:
   - Connected Devices
   - Network Traffic
   - Active Connections
   - Bandwidth Usage

### Using Network Tools

1. Scroll down to the "Network Tools" section
2. Available tools include:
   - Ping
   - Traceroute
   - DNS Lookup
   - Port Scanner
3. Select a tool and enter the required parameters
4. Click the corresponding button to run the tool
5. Results will be displayed below 