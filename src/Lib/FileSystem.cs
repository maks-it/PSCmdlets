/**
 * @license
 * Internet Systems Consortium license
 *
 * Copyright (c) 2020 Maksym Sadovnychyy (MAKS-IT)
 * Website: https://maks-it.com
 * Email: commercial@maks-it.com
 *
 * Permission to use, copy, modify, and/or distribute this software for any purpose
 * with or without fee is hereby granted, provided that the above copyright notice
 * and this permission notice appear in all copies.
 *
 * THE SOFTWARE IS PROVIDED "AS IS" AND THE AUTHOR DISCLAIMS ALL WARRANTIES WITH
 * REGARD TO THIS SOFTWARE INCLUDING ALL IMPLIED WARRANTIES OF MERCHANTABILITY AND
 * FITNESS. IN NO EVENT SHALL THE AUTHOR BE LIABLE FOR ANY SPECIAL, DIRECT,
 * INDIRECT, OR CONSEQUENTIAL DAMAGES OR ANY DAMAGES WHATSOEVER RESULTING FROM LOSS
 * OF USE, DATA OR PROFITS, WHETHER IN AN ACTION OF CONTRACT, NEGLIGENCE OR OTHER
 * TORTIOUS ACTION, ARISING OUT OF OR IN CONNECTION WITH THE USE OR PERFORMANCE OF
 * THIS SOFTWARE.
 */

using System.Collections.Generic;
using System.Linq;
using System.IO;
using Lib.ExtensionMethods;

namespace Lib
{
    /*
        Main FileSystem class
        Provides basic helper method to work with file system
    */
    /// <summary>
    /// Main <c>FileSystem</c> class.
    /// Provides basic helper method to work with file system.
    /// </summary>
    public class CMFileSystem
    {
        // Copy the file or folder's content to the specific folder
        /// <summary>
        /// Copy the file or folder's content to the specific folder.
        /// </summary>
        /// <param name="sourcePath">File or directory path</param>
        /// <param name="destDirPath">Destination directory</param>
        public static void CopyToFolder (string sourcePath, string destDirPath, bool overwrite = false)
        {
            if (!Directory.Exists(destDirPath))
                Directory.CreateDirectory(destDirPath);

            FileAttributes attr = File.GetAttributes(sourcePath);

            if (attr.HasFlag(FileAttributes.Directory))
            {
                foreach (string filePath in Directory.GetFiles(sourcePath, "*.*", SearchOption.AllDirectories))
                {
                    string destFilePath = Path.Combine(destDirPath, filePath.Replace(sourcePath, "").Trim('\\'));
                    string destDirectoryPath = Directory.GetParent(destFilePath).FullName;

                    if (!Directory.Exists(destDirectoryPath))
                        Directory.CreateDirectory(destDirectoryPath);

                    File.Copy(filePath, destFilePath, overwrite);
                }
            }
            else
            {
                //is the file
                File.Copy(sourcePath, Path.Combine(destDirPath, Path.GetFileName(sourcePath)), overwrite);
            }
        }

        // Deletes file or directory into specified path
        /// <summary>
        /// Deletes file or directory into specified path.
        /// </summary>
        /// <param name="itemPath">File or directory path</param>
        public static void Delete (string itemPath)
        {
            if (File.Exists(itemPath)) {
                File.Delete(itemPath);
            }

            if (Directory.Exists(itemPath)) {
                Directory.Delete(itemPath);
            }
        }

        // Resolve path with wildcards and returns all possible variants found
        /// <summary>
        /// Resolve path with wildcards and returns all possible variants found
        /// <para>If someone can do that better, than me, you're welcome!!! XD</para>
        /// </summary>
        /// <example>
        /// <code>
        /// List{T} paths = ResolveWildcardedPath(@"?:\Users\*\AppData\Roaming\SAP*");
        /// </code>
        /// </example>
        /// <param name="wildcardedPath">Example - @"?:\Users\*\AppData\Roa*\"</param>
        /// <returns>Returns all possible, but existing path variants found</returns>
        public static List<string> ResolveWildcardedPath(string wildcardedPath)
        {
            List<string> response = new List<string>();

            wildcardedPath = wildcardedPath.TrimEnd('\\');

            //determination if start the logic or not
            if (wildcardedPath.Contains("*") || wildcardedPath.Contains("?"))
            {
                List<string> pathsCollection = new List<string>();

                foreach (string item in wildcardedPath.Split('\\'))
                {
                    if (item == "?:")
                    {
                        foreach (DriveInfo drive in DriveInfo.GetDrives())
                        {
                            if(drive.ToString().Like(item + '\\'))
                                pathsCollection.Add(drive.ToString() );
                        }
                    }
                    else if (item.Contains("*") || item.Contains("?"))
                    {
                        List<string> temp = new List<string>();

                        foreach (string path in pathsCollection)
                        {
                            if (Directory.Exists(path))
                            {
                                FileAttributes attr = File.GetAttributes(path);
                                if (attr.HasFlag(FileAttributes.Directory))
                                {
                                    try
                                    {
                                        //collect all directories
                                        foreach (string file in Directory.GetFiles(path))
                                        {
                                            if (Path.GetFileName(file).Like(item))
                                                temp.Add(file + "\\");
                                        }
                                        //collect all files
                                        foreach (string dir in Directory.GetDirectories(path))
                                        {
                                            if (Path.GetFileName(dir).Like(item))
                                                temp.Add(dir + "\\");
                                        }
                                    }
                                    catch {
                                    }
                                }
                            }
                        }

                        pathsCollection = temp;
                    }
                    else
                    {
                        if (pathsCollection.Count == 0) {
                            pathsCollection.Add(item + "\\");
                        }
                        else {
                            for (int i = 0; i < pathsCollection.Count; i++) {
                                pathsCollection[i] += item + "\\";
                            }
                        }
                    }
                }

                //just trim each path
                pathsCollection = pathsCollection.Select(s => s.Trim('\\')).ToList();



                //validate all paths against wildcardedPath and after if it exists at all
                List<string> validResponse = new List<string>();


                string[] temp_wildcardedPath = wildcardedPath.Split('\\');

                foreach (string path in pathsCollection)
                {
                    string[] temp_path = path.Split('\\');
                    if (temp_wildcardedPath.Count() == temp_path.Count()) {
                        bool isValid = true;
                        for (int i = 0; i < temp_wildcardedPath.Count(); i++) {
                            if (!temp_wildcardedPath[i].Contains("*") && !temp_wildcardedPath[i].Contains("?") && temp_wildcardedPath[i] != temp_path[i]) {
                                isValid = false;
                                break;
                            }
                        }

                        if (isValid && (Directory.Exists(path) || File.Exists(path)))
                            validResponse.Add(path);

                    }
                }

                response = validResponse;
            }
            else
            {
                //if no wildcards found, leave path as is

                //if(Directory.Exists(wildcardedPath) || File.Exists(wildcardedPath))
                    response.Add(wildcardedPath);
            }

            return response;
        }

        // Test file name for duplicates, if is duplicate, new name is assigned
        /// <summary>
        /// Test file name for duplicates, if is duplicate, new name is assigned.
        /// </summary>
        /// <param name="fullPath">file path to test for duplicate</param>
        /// <returns>Returns updated file name</returns>
        public static string DuplicateFileNameCheck(string fullPath)
        {
            string fileNameOnly = Path.GetFileNameWithoutExtension(fullPath);
            string extension = Path.GetExtension(fullPath);
            string path = Path.GetDirectoryName(fullPath);
            string newFullPath = fullPath;

            int count = 1;
            while (File.Exists(newFullPath))
            {
                string tempFileName = string.Format("{0}({1})", fileNameOnly, count++);
                newFullPath = Path.Combine(path, tempFileName + extension);
            }

            return newFullPath;
        }
    }
}
