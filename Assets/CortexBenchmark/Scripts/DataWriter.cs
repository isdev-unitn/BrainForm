/*** Copyright Disclaimer
@Author: Michele Romani
@Website: http://bromans.github.io
@Email: michele.romani.zaltieri@gmail.com
@Date: 2024-06-01
This code is distributed under the CC License: any use of the code should provide credit to the author and a link to the original source.
Research and non-commercial use is allowed. Any commercial use must be agreed upon with the author.
***/

using System;
using System.IO;
using UnityEngine;
using System.Globalization;

namespace CortexBenchmark
{
    public class DataWriter : MonoBehaviour
    {
        private enum FileExtension
        {
            CSV,
            TXT
        }

        public enum WriterType
        {
            PerformanceMetrics,
        }

        [SerializeField]
        [Tooltip("The file extension of the output file.")]
        private FileExtension _fileExtension = FileExtension.CSV;

        [SerializeField]
        private string _filepath = "Benchmark/Output/";

        [SerializeField]
        private string _filename = "output";

        private WriterType _writerType;
        private StreamWriter _scoreWriter;

        private StreamWriter _experimentWriter;

        public void Initialize(WriterType writerType)
        {
            _writerType = writerType;
            OpenFile(_filepath, _filename);
            WriteHeader();
        }

        // Update is called once per frame
        void OnApplicationQuit()
        {
            CloseFile(_scoreWriter);
            CloseFile(_experimentWriter);
        }

        private string GetFileExtension(FileExtension fileExt)
        {
            string ext = "";
            switch (fileExt)
            {
                case FileExtension.CSV:
                    ext = ".csv";
                    break;
                case FileExtension.TXT:
                    ext = ".txt";
                    break;
                default:
                    ext = ".csv";
                    break;
            }

            return ext;
        }

        public void OpenFile(string filepath, string filename)
        {
            if (!Directory.Exists(filepath))
            {
                Directory.CreateDirectory(filepath);
            }
            if (_scoreWriter == null)
                _scoreWriter = new StreamWriter(Path.Combine(filepath, filename + "_"
                + _writerType.ToString()
                + "_score_" + DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss")
                + GetFileExtension(_fileExtension)));
            if (_experimentWriter == null)
                _experimentWriter = new StreamWriter(Path.Combine(filepath, filename + "_"
                + _writerType.ToString()
                + "_exp_"
                + DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss")
                + GetFileExtension(_fileExtension)));
        }

        public void CloseFile(StreamWriter writer)
        {
            if (writer != null)
            {
                writer.Close();
                writer.Dispose();
                writer = null;
            }
        }

        public void Flush()
        {
            _scoreWriter.Flush();
            _experimentWriter.Flush();
        }

        public void WritePerformanceScore(PerformanceMetrics.TaskScore score)
        {
            if (_scoreWriter != null)
            {
                _scoreWriter.WriteLine(
                score._startTime + "," +
                score._stopTime + "," +
                score._nClasses + "," +
                score._correctlyClassified + "," +
                score._missClassified + "," +
                score._itr.ToString("G3", CultureInfo.InvariantCulture) + "," +
                score._accuracy.ToString("G3", CultureInfo.InvariantCulture) + "," +
                score._elapsedTime.ToString("G3", CultureInfo.InvariantCulture));
                _scoreWriter.Flush();
            }
            
        }

        public void WriteTaskData(DateTime timestamp, int nClasses, int classId, int cue)
        {
            if (_experimentWriter != null)
            {
                _experimentWriter.WriteLine(
                nClasses + "," +
                timestamp + "," +
                classId + "," +
                cue);
                _experimentWriter.Flush();
            }
        }

        public void WriteHeader()
        {
            if (_scoreWriter != null && _experimentWriter != null)
            {
                _experimentWriter.WriteLine("current_ts,n_classes,current_class,current_cue");
                _scoreWriter.WriteLine("start_ts,stop_ts,n_classes,corr_classified,miss_classified,itr,accuracy,elapsed_time_s");
            }
        }
    }
}