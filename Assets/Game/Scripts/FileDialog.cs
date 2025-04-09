using System;
using System.IO;
using System.Runtime.InteropServices;
using UnityEngine;

public class FileDialog : MonoBehaviour
{
    public enum Mode { Open, Save }

    public string ShowDialog(string title, string defaultPath, string extension, Mode mode)
    {
        string path = string.Empty;
        
#if UNITY_EDITOR
        path = mode == Mode.Open ? 
            UnityEditor.EditorUtility.OpenFilePanel(title, defaultPath, extension) :
            UnityEditor.EditorUtility.SaveFilePanel(title, defaultPath, "NewConfig", extension);
#elif UNITY_STANDALONE_WIN
        path = ShowWindowsDialog(title, defaultPath, extension, mode);
#endif

        if (!string.IsNullOrEmpty(path))
        {
            Debug.Log((mode == Mode.Open ? "Selected" : "Saved") + " file: " + path);
        }
        else
        {
            Debug.Log("Operation canceled by user");
        }

        return path;
    }

#if UNITY_STANDALONE_WIN
    private string ShowWindowsDialog(string title, string defaultPath, string extension, Mode mode)
    {
        OPENFILENAME ofn = new OPENFILENAME();
        ofn.structSize = Marshal.SizeOf(ofn);
        ofn.filter = $"{extension} files\0*.{extension}\0All files\0*.*\0\0";
        ofn.file = new string(new char[256]);
        ofn.maxFile = ofn.file.Length;
        ofn.title = title;
        ofn.initialDir = defaultPath;
        ofn.defExt = extension;

        bool success = mode == Mode.Open ? 
            GetOpenFileName(ref ofn) : 
            GetSaveFileName(ref ofn);

        return success ? ofn.file : null;
    }

    [DllImport("Comdlg32.dll", CharSet = CharSet.Auto)]
    private static extern bool GetOpenFileName(ref OPENFILENAME ofn);

    [DllImport("Comdlg32.dll", CharSet = CharSet.Auto)]
    private static extern bool GetSaveFileName(ref OPENFILENAME ofn);

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    private struct OPENFILENAME
    {
        public int structSize;
        public IntPtr dlgOwner;
        public IntPtr instance;
        public string filter;
        public string customFilter;
        public int maxCustFilter;
        public int filterIndex;
        public string file;
        public int maxFile;
        public string fileTitle;
        public int maxFileTitle;
        public string initialDir;
        public string title;
        public int flags;
        public short fileOffset;
        public short fileExtension;
        public string defExt;
        public IntPtr custData;
        public IntPtr hook;
        public string templateName;
        public IntPtr reservedPtr;
        public int reservedInt;
        public int flagsEx;
    }
#endif
}