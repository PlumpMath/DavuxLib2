﻿/*
 * Author: David Amenta
 * Release Date: 12/12/2009
 * License: Free for any use.
 */


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.IO;
using System.Diagnostics;

namespace DavuxLib2
{
    public class Plugin
    {
        public static List<T> LoadPlugin<T>(string filename)
        {
            List<T> ret = new List<T>();
            if (File.Exists(filename))
            {
                Assembly assembly = Assembly.LoadFile(filename);
                foreach (Type type in assembly.GetTypes())
                {
                    if (!type.IsAbstract)
                    {
                        try
                        {
                            if (typeof(T).IsAssignableFrom(type))
                            {
                                T temp = (T)Activator.CreateInstance(type);
                                ret.Add(temp);
                                Trace.Write(temp.ToString() + " ");
                            }
                        }
                        catch (MissingMethodException ex)
                        {
                            Trace.WriteLine("Cannot load plugin " + type.FullName + ": " + ex.Message);
                        }
                        catch (Exception)
                        {
                            // incorrect type, likely to happen often.
                        }
                    }
                }
            }
            return ret;
        }

        public static List<T> Load<T>()
        {
            return Load<T>(System.Windows.Forms.Application.StartupPath);
        }

        public static List<T> Load<T>(string folder)
        {
            try
            {
                List<T> Plugins = new List<T>();
                if (Directory.Exists(folder))
                {
                    foreach (string f in Directory.GetFiles(folder))
                    {
                        FileInfo fi = new FileInfo(f);
                        if (fi.Extension.Equals(".dll") || fi.Extension.Equals(".exe"))
                        {
                            try
                            {
                                Trace.Write("--> " + fi.Name + " ");
                                Plugins.AddRange(LoadPlugin<T>(f));
                                Trace.WriteLine("[OK]");
                            }
                            catch (Exception ex)
                            {
                                Trace.WriteLine("[FAILED " + ex.Message + "]");
                            }
                        }
                    }
                }
                return Plugins;
            }
            catch (Exception ex)
            {
                Trace.WriteLine("Error loading plugins: " + ex);
            }
            return null;
        }
    }
}
