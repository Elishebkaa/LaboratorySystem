﻿using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using System.Windows.Media.Imaging;
using System.Linq;
using System.Net;
using System.Data.Entity;
using System.Windows.Controls;
using System.Collections.Generic;
using ZXing;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using System.IO;
using System.Drawing.Imaging;
using System.Drawing;

namespace LaboratorySystem
{
    public partial class LabAssistantWindow : Window
    {
        private DispatcherTimer sessionTimer;
        private TimeSpan sessionTime = TimeSpan.FromMinutes(150);
        private int userId;

        public static List<ServicesForOrder> list;

        public LabAssistantWindow(int userId, string name, string surname)
        {
            InitializeComponent();
            img.Source = new BitmapImage(new Uri(Environment.CurrentDirectory + "/images/laborant.jpeg"));
            this.userId = userId;
            tbUserName.Text = name + " " + surname;
            SessionTimer();
            dgOrders.ItemsSource = new Entities().ServicesForOrder.ToList();
        }
        


        //таймер
        private void SessionTimer()
        {
            sessionTimer = new DispatcherTimer();
            sessionTimer.Interval = TimeSpan.FromSeconds(1);
            sessionTimer.Tick += SessionTimer_Tick;
            sessionTimer.Start();
        }

        private void SessionTimer_Tick(object sender, EventArgs e)
        {
            sessionTime = sessionTime.Subtract(TimeSpan.FromSeconds(1));
            tbTimer.Text = sessionTime.ToString(@"hh\:mm\:ss");

            if (sessionTime <= TimeSpan.Zero)
            {
                sessionTimer.Stop();
                MessageBox.Show("Время сеанса истекло. Система будет заблокирована на 30 минут.");
                AddToHistoryEnd(userId);
                new MainWindow().Show();
                Close();               
            }
            else if (sessionTime <= TimeSpan.FromMinutes(15))
            {
                tbTimer.Foreground = System.Windows.Media.Brushes.Red;
                if (sessionTime.Seconds == 0 && sessionTime.Minutes == 15)
                {
                    MessageBox.Show("До окончания сеанса осталось 15 минут");
                }
            }
        }

        private void BtnGenerateReport_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Пардон не сделал(");
        }
        //при закрытии окна
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            sessionTimer?.Stop();
        }


        private void AddToHistoryEnd(int userId)
        {
            try
            {
                using (var db = new Entities())
                {
                    var history = new HistoryIn
                    {
                        id_user = userId,
                        lastenter = DateTime.Now,
                        ip = GetLocalIPAddress(),
                        id_Reason = 1
                    };

                    db.HistoryIn.Add(history);
                    db.SaveChanges();
                }
            }
            catch { }
        }

        private void AddToHistory(int userId)
        {
            try
            {
                using (var db = new Entities())
                {
                    var history = new HistoryIn
                    {
                        id_user = userId,
                        lastenter = DateTime.Now,
                        ip = GetLocalIPAddress(),
                        id_Reason = 3
                    };

                    db.HistoryIn.Add(history);
                    db.SaveChanges();
                }
            }
            catch { }
        }

        
        private string GetLocalIPAddress()
        {
            string ip;
            if (Dns.GetHostAddresses(Dns.GetHostName()).Length > 0)
            {
                ip = Dns.GetHostAddresses(Dns.GetHostName())[0].ToString();
            }
            else ip = "10.11.93.111";
            return ip;
        }

        private void BtnLogout_Click(object sender, RoutedEventArgs e)
        {
            AddToHistory(userId);
            new MainWindow().Show();
            Close();
        }


        /// <summary>
        /// Редактирование
        /// </summary>

        private void Hyperlink_Click(object sender, RoutedEventArgs e)
        {
            var db = new Entities();

            var dataCheck = ((Button)(sender)).DataContext as ServicesForOrder;
            list = db.ServicesForOrder.Where(x => x.id_order == dataCheck.id_order).ToList();
            EditServices editServices = new EditServices();
            editServices.Show();
        }

        

        private void BtnRefreshUsers_Click(object sender, RoutedEventArgs e)
        {
            var db = new Entities();
            dgOrders.ItemsSource = db.ServicesForOrder.ToList();
        }

        private void CreateOrder_Click(object sender, RoutedEventArgs e)
        {
            CreateOrder createOrder = new CreateOrder();
            createOrder.Show();
        }



        //Генерация штрихкода
        private void GenerateButton_Click(object sender, RoutedEventArgs e)
        {

            var dataCheck = ((Button)(sender)).DataContext as ServicesForOrder;
            int tubeCode = dataCheck.tube_code.Value;

            if (tubeCode == 0)
            {
                MessageBox.Show("Код пробирки не найден");
                return;
            }

            string PdfName = "Barcode.pdf";
            GenerateBarcodePdf(tubeCode, PdfName);
            MessageBox.Show("PDF с штрих-кодом успешно создан!");
        }

        //Штрихкод
        private void GenerateBarcodePdf(int tubeCode, string PdfPath)
        {
            using (PdfDocument document = new PdfDocument())
            {
                PdfPage page = document.AddPage();
                XGraphics gfx = XGraphics.FromPdfPage(page);

                BarcodeWriter barcodeWriter = new BarcodeWriter
                {
                    Format = BarcodeFormat.CODE_128,
                    Options = new ZXing.Common.EncodingOptions
                    {
                        Height = 100,
                        Width = 300,
                        Margin = 10
                    }
                };

                using (Bitmap barcodeBitmap = barcodeWriter.Write(tubeCode.ToString()))
                {
                    using (MemoryStream stream = new MemoryStream())
                    {
                        barcodeBitmap.Save(stream, ImageFormat.Png);
                        stream.Position = 0;
                        XImage pdfBarcodeImage = XImage.FromStream(stream);
                        gfx.DrawImage(pdfBarcodeImage, 50, 100, pdfBarcodeImage.PixelWidth, pdfBarcodeImage.PixelHeight);
                    }
                }
                document.Save(PdfPath);
            }
        }
    }
}