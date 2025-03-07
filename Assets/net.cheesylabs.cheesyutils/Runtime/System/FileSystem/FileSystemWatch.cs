using System.IO;
using UnityEngine;

// watches specified folder changes in the filesystem
// "Listens to the file system change notifications and raises events when a directory, or file in a directory, changes."
// references: http://stackoverflow.com/questions/15017506/using-filesystemwatcher-to-monitor-a-directory and http://www.c-sharpcorner.com/article/monitoring-file-system-using-filesystemwatcher-class-part1/

public class FileSystemWatch : MonoBehaviour
{
    private const string MyPath = "c:\\myfolder\\";
    private FileSystemWatcher _watcher;

    private void Start()
    {
        InitFileSystemWatcher();
    }

    private void InitFileSystemWatcher()
    {
        _watcher = new FileSystemWatcher();
        _watcher.Path = MyPath;
        _watcher.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.DirectoryName;
        _watcher.Filter = "*.*";

        //Handler for Changed Event
        _watcher.Changed += new FileSystemEventHandler(FileChanged);
        //Handler for Created Event
        _watcher.Created += new FileSystemEventHandler(FileCreated);
        //Handler for Deleted Event
        _watcher.Deleted += new FileSystemEventHandler(FileDeleted);
        //Handler for Renamed Event
        _watcher.Renamed += new RenamedEventHandler(FileRenamed);

        _watcher.EnableRaisingEvents = true;
    }

    private static void FileChanged(object source, FileSystemEventArgs e)
    {
        Debug.Log("FileChanged:" + e.FullPath);
    }

    private static void FileCreated(object source, FileSystemEventArgs e)
    {
        Debug.Log("FileCreated:" + e.FullPath);
    }

    private static void FileDeleted(object source, FileSystemEventArgs e)
    {
        Debug.Log("FileDeleted:" + e.FullPath);
    }

    private static void FileRenamed(object source, FileSystemEventArgs e)
    {
        Debug.Log("FileChanged:" + e.FullPath);
    }
}
