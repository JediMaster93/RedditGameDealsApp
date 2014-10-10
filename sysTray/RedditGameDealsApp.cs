using System;
using System.Drawing;
using System.Net;
using System.Windows.Forms;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Threading;


namespace sysTray
{
    /*
     *This class handles gui elements and "main loops",
     *most of the logic is behind DealController.
     * 
     * 
     */
    public class SysTrayApp : Form
    {
        [STAThread]
        public static void Main()
        {
            Application.Run(new SysTrayApp());
        }

        private NotifyIcon trayIcon;
        private ContextMenu trayMenu;
        private Button b;
        private System.Timers.Timer updateTimer;
        private int updateTimerInterval;
        private delegate void ballonClickDelegate(Object sender, EventArgs e);
        private DealController dealController;
        public SysTrayApp()
        {
            initialiseObjects();



        }
        private void initialiseObjects()
        {
            // Create a simple tray menu with only one item.
            trayMenu = new ContextMenu();
            trayMenu.MenuItems.Add("Exit", OnExit);

            // Create a tray icon.
            trayIcon = new NotifyIcon();
            trayIcon.Text = "MyTrayApp";
            trayIcon.Icon = new Icon(SystemIcons.Application, 40, 40);
            // Add menu to tray icon and show it.
            trayIcon.ContextMenu = trayMenu;
            trayIcon.Visible = true;

            //initialising my objects
            b = new Button();
            b.Text = "display best deal"; //temporary testing 
            b.Click += button_click;

            //pass in trayIcon that dealcontroller uses to display ballons
            dealController = new DealController(trayIcon);

            updateTimerInterval = 5 * 1000;
            updateTimer = new System.Timers.Timer(updateTimerInterval);
            updateTimer.Elapsed += dealController.displayBestDeal;

            updateTimer.AutoReset = true;
            updateTimer.Start();

            TableLayoutPanel p = new TableLayoutPanel();
            p.Name = "TableLayoutPanel";
            p.RowCount = 10;
            p.ColumnCount = 2;
            this.Controls.Add(p);
            Options o = new Options(this,p);
            o.addOptionsToWindow();

            o.subscribeObserver(dealController);


        }

        protected override void OnLoad(EventArgs e)
        {
            Visible = true; // Hide form window.
            this.Controls.Add(b);
            ShowInTaskbar = false; // Remove from taskbar.


            base.OnLoad(e);
        }

        private void OnExit(object sender, EventArgs e)
        {
            Application.Exit();
        }
        private void button_click(Object sender, EventArgs e)
        {
            //displayBestDeal(sender, e);


        }






        protected override void Dispose(bool isDisposing)
        {
            if (isDisposing)
            {
                // Release the icon resource.
                trayIcon.Dispose();
            }

            base.Dispose(isDisposing);
        }

    }
}
 