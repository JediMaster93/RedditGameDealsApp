using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace sysTray
{
    //dynamicaly renders and handles options based on a dictionary of options
    //sends dictionary of options to classes that have those options/care about that data.

    
    class Options : Observable
    {
        private Dictionary<String, Object> dictionary; //naming things is hard.. 
        Form optionsWindow;
        TableLayoutPanel panel;
        private Button submitButton;
        private List<Observer> observers;
        public Options(Form optionsWindow, TableLayoutPanel panel)
        {
            observers = new List<Observer>();
            dictionary = new Dictionary<string, object>();
            this.optionsWindow = optionsWindow;
            this.panel = panel;
            submitButton = new Button();
            submitButton.Text = "Submit";
            submitButton.Click += onSubmitButtonClick;

            //temporary dict iniatlisiation
            int i = 10;

            dictionary.Add("url", "http://www.reddit.com/r/videos/top/.json?limit=100&sort=today");
           
        }

        public void addOptionsToWindow()
        {
            int i = 0;
            foreach(KeyValuePair<String, Object> kvp in dictionary)
            {
                Label l = new Label();
                l.Text = kvp.Key;

                TextBox tb = new TextBox();
                tb.Text = "Input here";

                this.panel.Controls.Add(l);
                this.panel.Controls.Add(tb);



                //dictionary[kvp.Key] = "hi";

            }
            panel.Controls.Add(submitButton);


        }

        public void addOption(KeyValuePair<String, Object> option)
        {
            //get stuff from textboxes and change values to them...
            this.dictionary.Add(option.Key, option.Value);

        }
        private void onSubmitButtonClick(Object sender, EventArgs e)
        {

            notifyObservers();
            
        }
        private Object convertString(Object target,  String input)
        {
            if(target is int)
            {
                return Convert.ToInt32(input);
            }
            else if (target is String || target is string)
            {

                return input;
            }
            else
            {
                return null;

            }
        }

        //methods for  observer pattern

        public void subscribeObserver(Observer o)
        {
            observers.Add(o);
        }
        public void unsubscribeObserver(Observer o)
        {
            observers.Remove(o);

        }
        public void notifyObservers()
        {
            //pushes dictionary to all interested classes
            foreach(Observer o in observers)
            {

                o.update(this, dictionary);
            }
        }




    }
}
