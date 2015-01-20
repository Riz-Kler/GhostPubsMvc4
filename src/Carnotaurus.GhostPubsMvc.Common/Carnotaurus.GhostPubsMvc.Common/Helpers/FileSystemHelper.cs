using System;
using System.IO;
using Carnotaurus.GhostPubsMvc.Common.Extensions;

namespace Carnotaurus.GhostPubsMvc.Common.Helpers
{
    public class FileSystemHelper
    {
        public static void DeleteDirectory(string targetDir)
        {
            var files = Directory.GetFiles(targetDir);

            var dirs = Directory.GetDirectories(targetDir);

            foreach (var file in files)
            {
                File.SetAttributes(file, FileAttributes.Normal);
                File.Delete(file);
            }

            foreach (var dir in dirs)
            {
                DeleteDirectory(dir);
            }

            Directory.Delete(targetDir, false);
        }

        public static void EnsureFolders(string path, bool isRedirectional)
        {
            CreateFolders(path, isRedirectional);

            DeleteFolders(path, isRedirectional);

            CreateFolders(path, isRedirectional);
        }

        public static void DeleteFolders(String path, Boolean isRedirectional)
        {
            DeleteDirectory(path.ToLower());
        }

        public static void CreateFolders(String path, Boolean isRedirectional)
        {
            Directory.CreateDirectory(path.ToLower());
        }

        public static void WriteFile(string fullFilePath, string contents)
        {
            File.WriteAllText(fullFilePath.ToLower(), contents);
        }

        public static void CopyFolder(String source, String target)
        {
            var files = Directory.GetFiles(source);

            foreach (var file in files)
            {
                var fileInfo = new FileInfo(file);

                var destination = string.Format("{0}\\{1}", target, fileInfo.Name);

                fileInfo.CopyTo(destination);
            }

        }
    }
}