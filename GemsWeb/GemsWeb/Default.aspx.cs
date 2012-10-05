﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using GemsWeb.CustomControls;
using GemsWeb.Controllers;
using evmsService.entities;


namespace GemsWeb
{
    public partial class _default : System.Web.UI.Page
    {
        
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                RequestEvents(DateTime.Now, DateTime.Now.AddMonths(1));
            }
        }

        private void RequestEvents(DateTime start, DateTime end)
        {
            List<Events> events = new List<Events>();
            try
            {

                RegistrationClient client = new RegistrationClient();
                Events[] arrEventPublish = client.ViewEventForPublish(start, end);
                if (txtTag.Text.Trim() != "")
                {
                    arrEventPublish = client.ViewEventForPublishByDateAndTag(start, end, txtTag.Text.Trim());
                }

                client.Close();

                lstEvent.DataSource = arrEventPublish;

                lstEvent.DataValueField = "EventID";
                lstEvent.DataTextField = "Name";

                lstEvent.DataBind();

            }
            catch (Exception ex)
            {
                Alert.Show("Error Retreiving List of Events from Server", false, "~/Default.aspx");
                return;
            }

            
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            //Search and populate lstEvent
            RequestEvents(dpFrom.CalDate, dpTo.CalDate);
        }

        protected void lstEvent_SelectedIndexChanged(object sender, EventArgs e)
        {
            
            //try
            //{
            //    int eventID;
            //    if(!int.TryParse(lstEvent.SelectedValue, out eventID))
            //    {
            //        Alert.Show("Invalid Selection", false, "~/Default.aspx");
                
            //    }
            //    this.hypRegister.NavigateUrl = "~/Register.aspx?EventID=" + lstEvent.SelectedValue + "&Name=" + lstEvent.SelectedItem.Text;

            //    RegistrationClient client = new RegistrationClient();
            //    Publish publish = client.ViewPublish(eventID);
            //    Event event_ = client.GetEvent(eventID);
           
            //    client.Close();
            //    menuEvent.Visible = true;
            //    this.mvTab.Visible = true;
            //    lbleventname.Text = event_.Name;
            //    lbleventdate.Text = event_.StartDateTime.ToString("dd MMM yyyy");
            //    lbleventstarttime.Text = event_.StartDateTime.ToString("HH:mm");
            //    lbleventendtime.Text = event_.EndDateTime.ToString("HH:mm");
            //    lbleventdescription.Text = event_.Description;
            //    hypeventwebsite.Text = event_.Website;
            //    hypeventwebsite.NavigateUrl = event_.Website;
            //    if (publish != null)
            //        lbleventpublishinfo.Text = publish.Remarks;
            //    else
            //        lbleventpublishinfo.Text = "";
            //    Guest[] guests =  event_.Guests;
                
            //    gvGuest.DataSource = guests;
            //    gvGuest.DataBind();

            //    Program[] programs = event_.Programs;
            //    gvProgram.DataSource = programs;
            //    gvProgram.DataBind();
            //    //gvProgram.DataSource = programs;
            //    //gvProgram.bind
            //    //for (int i = 0; i < programs.Count(); i++)
            //    //{
            //    //    gvProgram.ro
            //    //}

            //   // gvGuest.Columns.Add(new DataControlField());

            //    if (publish == null || (publish.StartDateTime > DateTime.Now || publish.EndDateTime < DateTime.Now))

            //        this.hypRegister.Visible = false;
            //    else
                
            //        hypRegister.Visible = true;
                
             
  
            //}
            //catch (Exception ex)
            //{
            //    Alert.Show("Error Retreiving List of Events from Server", false, "~/Default.aspx");
            //    return;
            //}
        }

        protected void btnGO_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Event.aspx?EventID=" + lstEvent.SelectedValue);
        }
    }
}