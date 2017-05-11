using UnityEngine;
using System.IO;
using System;

public static class CSV {

    public static string[][] read(string pathCSV)
    {
        string[][] _data;
        string[] _lines;
        string[] _values;

        string CSV = ((TextAsset)Resources.Load(pathCSV)).text;

        if (!string.IsNullOrEmpty(CSV))
        {
            _lines = CSV.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);

            _data = new string[_lines.Length][];

            for (int i = 0; i < _lines.Length; i++)
            {
                _values = _lines[i].Split(new[] { ';' }/*, StringSplitOptions.RemoveEmptyEntries*/);

                if (_values.Length != 0)
                {
                    _data[i] = new string[_values.Length];

                    for (int j = 0; j < _values.Length; j++)
                    {
                        _data[i][j] = _values[j].Trim();
                    }
                }
                else
                {
                    _data[i] = new string[1] { "" };
                }

            }
            return _data;
        }
        else
        {
            throw new FileNotFoundException("CSV-File doesn't exist!");
        }

    }
}
