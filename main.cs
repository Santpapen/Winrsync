/*
 * Licensed under the Apache License, Version 2.0 (the "License");
 * 
Author: Roland Santiago
Date: 18/01/2018
Project_base: Apt_windows
Description: First version (Download pool files of the Debian repository) ftp.es.debian.org [line 27,122]
 */
using System;
using System.IO;
using System.IO.Compression;
using System.Net;


public class Apt_windows
{
    public static void Main()
    {
        Global data = new Global();
        Apt_windows apt = new Apt_windows();       
        apt.md5sumsDownload (); //Download Index File
        apt.readFile("md5sums"); 
        Console.ReadKey(); // Stopping Cmd and wait for input keyboard
    } // End Main

    public void md5sumsDownload(){
        string url = "http://ftp.es.debian.org/debian/indices/"; // index File path
        string filename = "md5sums.gz";
        Console.WriteLine ("Download index File,Please wait ...");
        FileDownload (url,filename);
        string directoryPath = @".";
        DirectoryInfo directorySelect = new DirectoryInfo(directoryPath);
        foreach (FileInfo fileToDecompress in directorySelect.GetFiles("*.gz"))
        {
            Decompress(fileToDecompress);
        }
    }
    public void FileDownload(string uri,string file){
        Global proxyData = new Global();
        string myStringWebResource = null;

        // create Webclient instance
        using (WebClient myWebClient = new WebClient()){
        
                  myStringWebResource = uri + file;

                // System Logon Credentials
                myWebClient.UseDefaultCredentials = true;
                myWebClient.Credentials = CredentialCache.DefaultCredentials;

            try
            {
                // Download File
                myWebClient.DownloadFile(myStringWebResource, file);
            }catch (WebException webEx)
            {
                Console.WriteLine("Error Http: {0}",webEx.Message);
                
            } // EndTryCatch
            
            } // End Web Request
    }
    public static void Decompress(FileInfo fileToDecompress)
    {
        using (FileStream originalFileStram = fileToDecompress.OpenRead())
        {
            string currentFileName = fileToDecompress.FullName;
            string newFileName = currentFileName.Remove(currentFileName.Length - fileToDecompress.Extension.Length); // Quita el valor .gz como nombre del Fichero
            using (FileStream decompressedFileStream = File.Create(newFileName))
            {
                using (GZipStream descompresionStream = new GZipStream(originalFileStram,CompressionMode.Decompress))
                {
                    descompresionStream.CopyTo(decompressedFileStream);
                    Console.WriteLine("Decompressed:{0}",fileToDecompress);
                }
            }
        } // End Using originalFileStream
        
    } // End Decompress

    public void readFile(string File)
        {
        Console.WriteLine("Starting files download,please wait ... \n You can have a cup of coffee or tea (Press intro);-)");
        Console.ReadKey();
        /* Read File Packages Directory */
        string[] dataPackage = new string[3];
      
        /*Function parse and read Packages_debian */
        try
        {
            using (StreamReader sr = new StreamReader("md5sums"))
            {
                string line;
                string downloadDir = null;
                while ((line = sr.ReadLine()) != null)
                {
                    bool aValue = line.Contains("pool/"); // Filter pool directory
                    if (aValue)
                    {
                        string[] items = line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries); // Parse doble space
                        int LastMax = items[1].Split('/').Length;
                        string[] Filename = items[1].Split('/');
                        for (int i = 0; i < LastMax - 1; i++)
                        {
                            downloadDir = downloadDir + Filename[i] + "/"; // build local path directory
                        }
                        string dbFilename = Filename[LastMax - 1];
           
                        if (!System.IO.Directory.Exists(downloadDir)) 
                        {
                            try
                            {
                                System.IO.Directory.CreateDirectory(downloadDir); // Create directory if !Exist
                            }
                            catch
                            {
                                continue;
                            }

                        }
                        // Download Files
                        string url = "http://ftp.es.debian.org/debian/";
                        string filename = items[1];
                        FileDownload(url, filename);
                        Console.WriteLine(dbFilename); // Output of Files Downloader
                        downloadDir = null;
                    } //pool Filter

                } // While ReadLine
  
            } // Open md5sums

        }
        catch (Exception e)
        {
            Console.WriteLine("Error Open File: {0}", e.Message);
        }
    } // End readFile
} // End Class
