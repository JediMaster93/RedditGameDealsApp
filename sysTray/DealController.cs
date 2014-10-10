using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace sysTray
{
    /**
     *Controls displaying, parsing, and sorting of deals 
     *displaying is done through trayIcon created in main class.
     * 
     * 
     **/
    class DealController : Observer
    {

        private WebClient webClient;
        private String topThreadsURL = "";
        private List<GameDeal> seenDeals;
        private NotifyIcon trayIcon;
        List<EventHandler> ballonClickEventList;
        private int scoreThreshold; //needs to be able to be defined by the user through the traymenu



        public  DealController(NotifyIcon trayIcon)
        {
            this.trayIcon = trayIcon;
            seenDeals = new List<GameDeal>();
            topThreadsURL = "http://www.reddit.com/r/gamedeals/top/.json?limit=100&sort=today";
            scoreThreshold = 50;
            webClient = new WebClient();
            ballonClickEventList = new List<EventHandler>();
        }

        public List<GameDeal> parseGameDeals()
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

        public Boolean isDealInSeenList(GameDeal deal)
        {
            //returns true if deal is in seendeal list
            foreach (GameDeal seenDeal in seenDeals)
            {
                if (deal.title.Equals(seenDeal.title))
                {
                    return true;
                }
            }

            return false;
        }

        public void displayBestDeal(Object sender, EventArgs e)
        {

            GameDeal dealToDisplay = findBestDeal();
            if (dealToDisplay != null)
            {

                displayDeal(dealToDisplay);
            }
        }
        public void displayDeal(GameDeal deal)
        {
            //removes previous events from trayicon, attaches new event, displays deal.
            ballonClickedUnsubscribeAllEvents();
            addDealToBallonEvents(deal);

            trayIcon.ShowBalloonTip(5000, "New Deal!", deal.title, ToolTipIcon.Info);


        }
        public void addDealToBallonEvents(GameDeal deal)
        {
            //add a deal to event list, event list is here so we can unsubscribe all events later.
            // probably shouldnt be a list...
            EventHandler handlerReference = (sender, eventargs) =>
            {
                System.Diagnostics.Process.Start("http://www.reddit.com" + deal.permalink);
                seenDeals.Add(deal);
            };
            ballonClickEventList.Add(handlerReference);
            trayIcon.BalloonTipClicked += handlerReference;
        }
        public void ballonClickedUnsubscribeAllEvents()
        {
            //remove all events from trayicon, if we dont do this 
            //when user clicks on ballon all prevous deals will be shown(because of previously attached events)
            foreach (EventHandler e in ballonClickEventList)
            {
                trayIcon.BalloonTipClicked -= e;
            }
        }

        public GameDeal findBestDeal()
        {
            //looks through current top deals, returns the best one that user has not already seen.
            List<GameDeal> deals = parseGameDeals();
            GameDeal bestDeal = null;
            foreach (GameDeal deal in deals)
            {
                if (deal.score >= scoreThreshold)
                {

                    if (bestDeal == null)
                    {
                        //best deal is not initalised, whatever this deal is, make it a best deal.
                        if (!isDealInSeenList(deal) && !deal.over_18)
                        {
                            bestDeal = deal;
                        }
                    }
                    else
                    {
                        //compare current deal in loop with best deal
                        //deal.over18 is a expired deal in  /r/gamedeals.
                        if (deal.score > bestDeal.score && !isDealInSeenList(deal) && !deal.over_18)
                        {
                            bestDeal = deal;
                        }
                    }
                }
            }
            return bestDeal;
        }

        public void update(Observable obs, Object arg)
        {
            if(arg is Dictionary<String, Object>)
            {
                //cast arg to dict
                Dictionary<String, Object> dict  = (Dictionary<String, Object>)arg;
                if (dict["url"] != null && dict["url"].GetType() == typeof(String))
                {
                    changeUrlAndFlushData((String)dict["url"]);
                }
                

            }

        }
        public void changeUrlAndFlushData(String url)
        {
            //we cant simply change url because of how ballons and algorithms for finding deals work.
            //this method takes care of that so we instatly change to new url as if we restarted the program.
            topThreadsURL = url;
            seenDeals = new List<GameDeal>();
            ballonClickedUnsubscribeAllEvents();

        }

    }



}
