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
        private String topThreadsURL = "http://www.reddit.com/r/gamedeals/top/.json?limit=100&sort=today";
        private WebClient webClient;
        private List<GameDeal> seenDeals = new List<GameDeal>();
        public SysTrayApp()
        {
            // Create a simple tray menu with only one item.
            trayMenu = new ContextMenu();
            trayMenu.MenuItems.Add("Exit", OnExit);
            
            // Create a tray icon. In this example we use a
            // standard system icon for simplicity, but you
            // can of course use your own custom icon too.
            trayIcon = new NotifyIcon();
            trayIcon.Text = "MyTrayApp";
            trayIcon.Icon = new Icon(SystemIcons.Application, 40, 40);
            // Add menu to tray icon and show it.
            trayIcon.ContextMenu = trayMenu;
            trayIcon.Visible = true;
           // trayIcon.ShowBalloonTip(5, "title", "text", ToolTipIcon.Info);

            //button
            b = new Button();
            b.Text = "update";
            b.Click += button_click;
            //web client
            webClient = new WebClient();

         //   trayIcon.ShowBalloonTip(5, "new_deal", "hello", ToolTipIcon.Info);
           // trayIcon.ShowBalloonTip(5, "new_deal", "hello again", ToolTipIcon.Info);
        //    trayIcon.ShowBalloonTip(1, "new_deal", "hello again", ToolTipIcon.Info);
            

            
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
            notifications_test();
          

        }
        private List<GameDeal> parseGameDeals()
        {
            // parses what api gives and returns a gamedeals array
            String data = webClient.DownloadString(topThreadsURL);
            JObject ob = JObject.Parse(data);

            JObject ss = (JObject)ob["data"];
            JArray arr = (JArray)ss["children"];

            GameDeal deal = JsonConvert.DeserializeObject<GameDeal>(arr[0]["data"].ToString());
            List<GameDeal> deals = new List<GameDeal>();

            foreach (JObject obj in arr)
            {
                deals.Add(JsonConvert.DeserializeObject<GameDeal>(obj["data"].ToString()));
            }
            return deals;

        }
        private void notifications_test()
        {
            //find deals with more than 200 score and display them
            List<GameDeal> deals = parseGameDeals();
            int i = 2000;
            List<System.Timers.Timer> tlist = new List<System.Timers.Timer>();
            GameDeal bestDeal = new GameDeal();

            foreach (GameDeal deal in deals)
            {
                if (deal.score >= 0)
                {
                    //this gives us ability to display-scrool trough all the possible list-games
                    //using events from timer i can put some items to never show again
                    //on click ->open item in browser
                    //MAybe, it would be actually really usefull to put the gamedeal into timer??
                    //
                    //SHOW HIGHEST DEAL FOR NOW (multiple deals dont work properly because of baloon behaviour)
                    if(deal.score > bestDeal.score && !isDealInSeenList(deal))
                    {
                        bestDeal = deal;
                    }
                   /* System.Timers.Timer t = new System.Timers.Timer();
                    t.Elapsed += (sender, eventargs) =>
                        {
                          //  b.Text = deal.title;
                            //once ballon behaviour is fixed i can work on multiple deals.
                            //right now, find biggest deal, display it.
                            trayIcon.ShowBalloonTip(1, "new_deal", deal.title, ToolTipIcon.Info);
                            trayIcon.BalloonTipClicked += (sender2, eventargs2) =>
                                {
                                    b.Text = deal.title;
                                    deal.seen = true;
                                    System.Diagnostics.Process.Start("http://www.reddit.com/" + deal.permalink);

                                };

                        };
                    t.Interval = i;
                    i += 2000;
                    t.Enabled = true;
                    tlist.Add(t);
                    t.AutoReset = false;
                    */

                    
                }


            }
            trayIcon.BalloonTipClicked += (sender, eventargs) =>
            {
                System.Diagnostics.Process.Start("http://www.reddit.com/" + bestDeal.permalink);
                seenDeals.Add(bestDeal);


            };

            trayIcon.ShowBalloonTip(5000, "New Deal!", bestDeal.title, ToolTipIcon.Info);
         
           /* foreach (System.Timers.Timer t in tlist)
            {
                t.Start();
            }*/
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

        private void ShowBalloonWindow(int timeout, string title, string text, ToolTipIcon icon)
        {
            if (timeout <= 0)
                return;

            int timeoutCount = 0;
            trayIcon.ShowBalloonTip(timeout, title, text, icon);

            while (timeoutCount < timeout)
            {
                Thread.Sleep(1);
                timeoutCount++;
            }
            
            trayIcon.Dispose();
        }

        private Boolean isDealInSeenList(GameDeal deal)
        {
            foreach(GameDeal seenDeal in seenDeals)
            {
                if(deal.title.Equals(seenDeal.title))
                {
                    return true;
                }
            }

            return false;
        }
    }
}