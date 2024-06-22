# FolderSync

FolderSync is a simple tool to synchronize two folders: source and replica, ensuring an identical copy of the source folder in the replica folder.

## Features

- One-way synchronization: replica folder mirrors exactly what is in the source folder.
- Periodic synchronization based on a specified interval.
- Logging of file creation, copying, and removal operations to a specified log file and console output.

## Requirements

- .NET Core (or .NET Framework)
- Git
  
## Usage

### Command Line Arguments

You can run the program with the following command-line arguments:

- `-source <source_path>`: Path to the source folder.
- `-replica <replica_path>`: Path to the replica folder.
- `-interval <interval_in_seconds>`: Synchronization interval in seconds.
- `-log <log_file_path>`: Path to the log file for synchronization operations.

## Example

To synchronize folders located at `C:\Source` and `D:\Replica` every 60 seconds, and log operations to `sync.log`:

```bash
dotnet run -- -source "C:\Source" -replica "D:\Replica" -interval 60 -log "sync.log"
