import socket  # Importing the socket module to create network sockets
import sys
# Defining the host and port to connect to
HOST = '127.0.0.1'  # Localhost IP address
PORT = '8080'  # Port number to connect to
# Creating a socket object for the client
client_socket = socket.socket(socket.AF_INET, socket.SOCK_STREAM)

# Connecting to the server at the specified host and port
client_socket.connect((HOST, int(PORT)))
# Sending a message to the server by encoding a string to bytes using UTF-8 encoding and sending it through the socket
res = "".join(sys.argv[1]).encode('utf-8')
client_socket.send(res)
# Receiving a response from the server by receiving up to 1024 bytes of data through the socket and decoding it to a string using UTF-8 encoding
print(client_socket.recv(16384))

# Closing the socket connection with the server
client_socket.close()