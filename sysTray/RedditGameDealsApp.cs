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
        private String topThreadsURL;
        private WebClient webClient;
        private List<GameDeal> seenDeals;
        private int scoreThreshold; //needs to be able to be defined by the user through the traymenu
        private System.Timers.Timer updateTimer;
        private int updateTimerInterval;
        private delegate void ballonClickDelegate(Object sender, EventArgs e);
        List<EventHandler> ballonClickEventList;
        private GameDeal GLOBAL_DEAL;

        /*
         * 
         *
         * */
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
 /*METHODS THAT MIGHT BE USEFUL LATER*/
/*private void displayBestDeal()
      {
          //find deals with more than 200 score and display them
          List<GameDeal> deals = parseGameDeals();
          int i = 2000;
          List<System.Timers.Timer> tlist = new List<System.Timers.Timer>();
          GameDeal bestDeal = new GameDeal();

          foreach (GameDeal deal in deals)
          {
              if (deal.score >= scoreThreshold)
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
          }
      }*/

/*  private void ShowBalloonWindow(int timeout, string title, string text, ToolTipIcon icon)
      {
          //actually shows a ballon tip for the timeout value(.net method doesnt do that, thanks microsoft)
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
      }*/