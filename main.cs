/*
Author: Roland Santiago
Fecha: 15/01/2018
Proyecto_base: Apt_windows
Descripción: 
 */
using System;
using System.IO;
 using System.IO.Compression;
using System.Net;


public class Apt_windows
{
    public static void Main()
    {
        Apt_windows apt = new Apt_windows();       
        apt.md5sumsDownload ();
        apt.readFile("md5sums");
        Console.ReadKey(); // Para el Cmd y se queda a la espera
    } // End Main
    public void md5sumsDownload(){
        string url = "http://ftp.es.debian.org/debian/indices/";
        string filename = "md5sums.gz";
        Console.WriteLine ("Descarga de archivo indices");
        FileDownload (url,filename);
        string directoryPath = @".";
        DirectoryInfo directorySelect = new DirectoryInfo(directoryPath);
        foreach (FileInfo fileToDecompress in directorySelect.GetFiles("*.gz"))
        {
            Decompress(fileToDecompress);
        }
    }
    public void FileDownload(string uri,string file){
        //string remoteUri ="https://www.alemansencillo.com/img/ns/";
        //string fileName = "logo.gif";
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
                Console.WriteLine(webEx.Message);
                
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
        Console.WriteLine("Lectura");
        /* Lectura del Fichero Package */
        string[] dataPackage = new string[3];
        // bool printData = false;
        /*Funcion de Parseo y Lectura de Packages_debian */
        try
        {
            using (StreamReader sr = new StreamReader("md5sums"))
            {
                string line;
                string downloadDir = null;
                while ((line = sr.ReadLine()) != null)
                {
                    bool aValue = line.Contains("pool/");
                    if (aValue)
                    {
                        // Console.WriteLine(line);
                        string[] items = line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                        int LastMax = items[1].Split('/').Length;
                        string[] Filename = items[1].Split('/');
                        for (int i = 0; i < LastMax - 1; i++)
                        {
                            downloadDir = downloadDir + Filename[i] + "/";
                        }
                        string dbFilename = Filename[LastMax - 1];
                        //Console.WriteLine(downloadDir);

                        if (!System.IO.Directory.Exists(downloadDir))
                        {
                            try
                            {
                                System.IO.Directory.CreateDirectory(downloadDir);
                            }
                            catch
                            {
                                continue;
                            }

                        }
                        string url = "http://ftp.es.debian.org/debian/";
                        string filename = items[1];
                        FileDownload(url, filename);
                        Console.WriteLine(dbFilename);
                        downloadDir = null;
                    } //pool Filter

                } // While ReadLine
  
            } // Apertura de Fichero

        }
        catch (Exception e)
        {
            Console.WriteLine("No es posible abrir el Fichero");
            Console.WriteLine(e.Message);
        }
    } // End readFile
} // End Class