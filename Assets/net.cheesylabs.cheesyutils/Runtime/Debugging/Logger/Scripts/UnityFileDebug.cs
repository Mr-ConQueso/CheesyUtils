using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using UnityEngine;

namespace CheesyUtils
{
    // short keynames are used to make json output small
    [Serializable]
    public class LogOutput
    {
        public string T;  //type
        public string TM; //time
        public string L;  //log
        public string S;  //stack
    }
    
    
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public enum FileType
    {
        CSV,
        JSON,
        TSV,
        TXT
    }

    [ExecuteInEditMode]
    public class UnityFileDebug : MonoBehaviour
    {
        public bool UseAbsolutePath;
        public string FileName = "Log";
        public FileType FileType = FileType.CSV;

        public string AbsolutePath = "c:\\";

        public string FilePath;
        public string FilePathFull;
        public int Count;

        private StreamWriter _fileWriter;

        private static string FileExtensionFromType(FileType type)
        {
            switch (type)
            {
                case FileType.JSON: return ".json";
                case FileType.CSV: return ".csv";
                case FileType.TSV: return ".tsv";
                case FileType.TXT:
                default: return ".txt";
            }
        }

        private void OnEnable()
        {
            UpdateFilePath();
            if (!Application.isPlaying) return;
            
            Count = 0;
            _fileWriter = new StreamWriter(FilePathFull, false);
            _fileWriter.AutoFlush = true;
            switch (FileType)
            {
                case FileType.CSV:
                    _fileWriter.WriteLine("type,time,log,stack");
                    break;
                case FileType.JSON:
                    _fileWriter.WriteLine("[");
                    break;
                case FileType.TSV:
                    _fileWriter.WriteLine("type\ttime\tlog\tstack");
                    break;
                case FileType.TXT:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            Application.logMessageReceived += HandleLog;
        }

        public void UpdateFilePath()
        {
            FilePath = UseAbsolutePath ? AbsolutePath : Application.persistentDataPath;
            FilePathFull = Path.Combine(FilePath, FileName + "." + DateTime.Now.ToString("yyyy.MM.dd.HH.mm.ss") + FileExtensionFromType(FileType));
        }

        private void OnDisable()
        {
            if (!Application.isPlaying) return;
            
            Application.logMessageReceived -= HandleLog;

            switch (FileType)
            {
                case FileType.JSON:
                    _fileWriter.WriteLine("\n]");
                    break;
                case FileType.CSV:
                case FileType.TSV:
                case FileType.TXT:
                default:
                    break;
            }
            _fileWriter.Close();
        }

        private void HandleLog(string logString, string stackTrace, LogType type)
        {
            LogOutput output = new LogOutput();
            if (type == LogType.Assert)
            {
                output.T = "Assert";
                output.L = logString;
            }
            else if (type == LogType.Exception)
            {
                output.T = "Exception";
                output.L = logString;
            }
            else
            {
                int end = logString.IndexOf("]", StringComparison.Ordinal);
                if (end > 1)
                {
                    output.T = logString.Substring(1, end - 1);
                    output.L = logString.Substring(end + 2);
                }
                else
                {
                    output.T = type.ToString();
                    output.L = logString;
                }
            }

            output.S = stackTrace;
            output.TM = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            switch (FileType)
            {
                case FileType.CSV:
                    _fileWriter.WriteLine(output.T + "," + output.TM + "," + output.L.Replace(",", " ").Replace("\n", "") + "," + output.S.Replace(",", " ").Replace("\n", ""));
                    break;
                case FileType.JSON:
                    _fileWriter.Write((Count == 0 ? "" : ",\n") + JsonUtility.ToJson(output));
                    break;
                case FileType.TSV:
                    _fileWriter.WriteLine(output.T + "\t" + output.TM + "\t" + output.L.Replace("\t", " ").Replace("\n", "") + "\t" + output.S.Replace("\t", " ").Replace("\n", ""));
                    break;
                case FileType.TXT:
                    _fileWriter.WriteLine("Type: " + output.T);
                    _fileWriter.WriteLine("Time: " + output.TM);
                    _fileWriter.WriteLine("Log: " + output.L);
                    _fileWriter.WriteLine("Stack: " + output.S);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            Count++;
        }
    }
}
