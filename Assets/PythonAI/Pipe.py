import time
import socket
import os
import Command


def pipe_server():
    print("Starting AI Server...")
    while True:
        try:
            host, port = "127.0.0.1", 25005
            global pipe
            pipe = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
            pipe.connect((host, port))
            while True: 
                try:
                    message = pipe.recv(1024).decode("UTF-8")
                    if message.lower() == "exit":
                        print("Received exit signal, shutting down...")
                        break  # Stop the server
                    Command.RunCommand(pipe,message)
                            
                except Exception as e:
                    print(f"Unity disconnected with {e}")
                    break

        except Exception as e:
            print(f"Pipe Error: {e}")
            time.sleep(1)  # Prevent instant retries


pipe_server()
