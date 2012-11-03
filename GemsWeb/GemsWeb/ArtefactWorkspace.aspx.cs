﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using evmsService.entities;
using GemsWeb.Controllers;

namespace GemsWeb
{
    public partial class ArtefactWorkspace : System.Web.UI.Page
    {
        string wrkSpaceDir = "WorkSpace\\";
        protected void Page_Init(object sender, EventArgs e)
        {
            Page.Form.Attributes.Add("enctype", "multipart/form-data");
            /*http://couldbedone.blogspot.com/2007/07/what-wrong-with-accordion-control.html*/
            x.FindControl("null");
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                //EventClient evclient = new EventClient();
                //lblSelectedFolder.Text = "-";
                ////Events ev_ = evclient.GetEvent(EventID());
                //Events[] evList = evclient.ViewEventsByTag(NUSNetUser(),"");
                //evclient.Close();
                //bool authorized = false;
                //for (int i = 0; i < evList.Count(); i++)
                //{
                //    if (evList[i].EventID==EventID())
                //    {
                //        authorized = true;
                //        break;
                //    }
                //}

                //if (!authorized)
                //{
                //    Alert.Show("You are not authorized to view this workspace!", true, "SelectEventWorkspace.aspx");
                //}
                //if (!evList.Contains(ev_))
                //{
                //    Alert.Show("You are not authorized to view this workspace!", true, "SelectEventWorkspace.aspx");
                //}
            }

            if (NUSNetUser() == null)
            {
                Alert.Show("Please Login First", true);
            }

            loadTreeView();
        }

        protected void treeWS_SelectedNodeChanged(object sender, EventArgs e)
        {
            btnResetFile_Click(sender, e);

            TreeNode tn = treeWS.SelectedNode;
            int level = getNodeLevel(tn.ValuePath);

            if (level == 0) // Event
            {

                lblSelectedFolder.Text = "-";
                loadFiles();
                return;
            }
            else
            {
                //Folder
                if (level == 1)
                {
                    lblSelectedFolder.Text = tn.Value.Trim();
                    loadFiles(tn.Value.Trim());
                }

            }
        }

        #region "Load"

        private double GetFolderSize(string DirPath, bool IncludeSubFolders = true)
        {
            //long lngDirSize = 0;
            ////FileInfo objFileInfo = default(FileInfo);
            //DirectoryInfo objDir = new DirectoryInfo(DirPath);
            //try {
            //    //add length of each file

            //    foreach (FileInfo objFileInfo in objDir.GetFiles()) {
            //        lngDirSize += objFileInfo.Length;
            //    }

            //    //call recursively to get sub folders
            //    //if you don't want this set optional
            //    //parameter to false 
            //    if (IncludeSubFolders) {
            //        foreach (DirectoryInfo objSubFolder in objDir.GetDirectories())
            //        {
            //            lngDirSize += GetFolderSize(objSubFolder.FullName);
            //        }
            //    }

            //} catch (Exception Ex) {
            //}

            //return lngDirSize;
            return 0;
        }

        private void loadTreeView()
        {
            EventClient evClient = new EventClient();
            Events event_ =null;
            try
            {
                event_= evClient.GetEvent(EventID());
            }
            catch (Exception)
            {
                evClient.Close();
                Alert.Show("You are not authorized to view this workspace!", true, "SelectEventWorkspace.aspx");
                return;
            }

            evClient.Close();

            treeWS.Nodes.Clear();
            TreeNode treeRoot = new TreeNode(event_.Name);
            treeRoot.ImageUrl = "~/images/folders.gif";
            loadFolders(treeRoot);

            treeWS.Nodes.Add(treeRoot);
            treeWS.ExpandAll();
        }

        private void loadFolders(TreeNode tn)
        {
            User u = NUSNetUser();
            if (u == null)
            {
                return;
            }
            ArtefactClient arclient = new ArtefactClient();
            WorkspaceFolders[] WrkSpace=null;
            try
            {
                WrkSpace = arclient.GetWorkSpaceFolders(NUSNetUser(), EventID());
            }
            catch (Exception)
            {
                arclient.Close();
                Alert.Show("You are not authorized to view this workspace!", true, "SelectEventWorkspace.aspx");
                return;
                //throw;
            }

            if (WrkSpace == null || WrkSpace.Count() == 0)
            {
                return;
            }

            foreach (WorkspaceFolders folder in WrkSpace)
            {
                TreeNode childNode = new TreeNode(folder.FolderName.ToString());
                childNode.ImageUrl = "~/images/folder.gif";
                childNode.ToolTip = folder.FolderDescription.ToString().Trim();
                tn.ChildNodes.Add(childNode);
            }
        }

        private void loadFiles(string folderName = "-")
        {
            ArtefactClient arclient = new ArtefactClient();
            WorkspaceFiles[] WrkSpaceFile = arclient.GetWorkSpaceFiles(NUSNetUser(), EventID(), folderName);
            gvFiles.DataSource = WrkSpaceFile;
            gvFiles.DataBind();
        }
        #endregion

        #region "Reset"
        protected void btnResetFile_Click(object sender, EventArgs e)
        {
            txtFileDesc.Text = "";
            txtFileURLExt.Text = "";
        }
        #endregion

        #region " Add "
        protected void btnAddFile_Click(object sender, EventArgs e)
        {
            lblMsg.Text = "";

            if (lblSelectedFolder.Text.Trim() != "-")
            {
                if (gvFiles.SelectedIndex < 0)
                {
                    if (!fuFileUpload.HasFile && txtFileURLExt.Text.Trim().Length == 0)
                    {
                        lblMsg.Text = "Please attach your file or specify the file name in the external url text field.";
                        return;
                    }
                    else if (fuFileUpload.HasFile && txtFileURLExt.Text.Trim().Length > 0)
                    {
                        lblMsg.Text = "Please only attach your file OR specify the file name in your external url, not both.";
                        return;
                    }
                }
            }
            else
            {
                lblMsg.Text = "You Cannot Upload file at root Folder!";
                return;
            }

            string filename = null;
            string fileurl = "";

            ArtefactClient arClient = new ArtefactClient();

            if (fuFileUpload.HasFile)
            {
                filename = fuFileUpload.FileName.Replace(" ", "_");
            }
            else
            {
                fileurl = txtFileURLExt.Text.Trim();
                filename = fileurl.Substring(fileurl.LastIndexOf("/") + 1);
            }

            try
            {
                if (hidFile.Value == "")
                {
                    //Upload New file                    
                    arClient.UploadFile(NUSNetUser(), EventID(), lblSelectedFolder.Text.Trim(), filename, txtFileDesc.Text.Trim(), fileurl);

                    if (fuFileUpload.HasFile)
                    {
                        UploadFile(filename);
                    }

                    loadFiles(lblSelectedFolder.Text.Trim());
                    lblMsg.Text = "Upload Success";
                }
                else
                {
                    WorkspaceFiles wrkFile = arClient.GetWorkSpaceFile(EventID(), lblSelectedFolder.Text.Trim(), hidFile.Value);

                    if (wrkFile.UploadedBy == NUSNetUser().UserID)
                    {
                        arClient.UpdateFile(NUSNetUser(), EventID(), lblSelectedFolder.Text.Trim(), hidFile.Value, txtFileDesc.Text.Trim(), fileurl);
                        lblMsg.Text = "Upload Success";
                    }
                    else
                    {
                        lblMsg.Text = "You cannot edit file that does not belong to you!";
                    }
                }

                txtFileDesc.Text = "";
                txtFileURLExt.Text = "";
                loadFiles(lblSelectedFolder.Text.Trim());
            }
            catch (Exception)
            {
                lblMsg.Text = "Upload Failed";
                throw;
            }

        }
        #endregion

        private int getNodeLevel(string path)
        {
            int x = path.Length - path.Replace("/", "").Length;
            return x;
        }

        protected void gvFiles_RowCommand(object sender, System.Web.UI.WebControls.GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Del")
            {
                int index = int.Parse(e.CommandArgument.ToString());

                string fileID = ((Label)gvFiles.Rows[index].Cells[2].FindControl("lblFileName")).Text.Replace(" ", "%20");

                string filepath = workSpaceDir(lblSelectedFolder.Text.Trim()) + "\\" + ((Label)gvFiles.Rows[index].Cells[2].FindControl("lblFileName")).Text;
                ArtefactClient arClient = new ArtefactClient();
                try
                {
                    WorkspaceFiles wrkFile = arClient.GetWorkSpaceFile(EventID(), lblSelectedFolder.Text.Trim(), fileID);

                    if (wrkFile.UploadedBy == NUSNetUser().UserID)
                    {
                        arClient.DeleteFile(NUSNetUser(), EventID(), lblSelectedFolder.Text.Trim(), fileID);

                        if (filepath.Contains("\\WorkSpace\\"))
                        {
                            if (System.IO.File.Exists(filepath))
                            {
                                System.IO.File.Delete(filepath);
                            }
                        }
                        lblMsg.Text = "File Successfully removed!";
                        txtFileDesc.Text = "";
                        txtFileURLExt.Text = "";
                        loadFiles(lblSelectedFolder.Text.Trim());
                    }
                    else
                    {
                        lblMsg.Text = "You can only delete what is belong to you!";
                    }
                }
                catch (Exception)
                {
                    lblMsg.Text = "File Failed to remove!";
                }
                finally
                {
                    arClient.Close();
                }


                //loadFiles(classId, folderID);
            }
        }

        protected void gvFiles_SelectedIndexChanged(object sender, EventArgs e)
        {
            hidFile.Value = ((Label)gvFiles.SelectedRow.Cells[2].FindControl("lblFileName")).Text;
            txtFileDesc.Text = ((Label)gvFiles.SelectedRow.Cells[2].FindControl("lblFileDesc")).Text;
        }

        #region "Page Global Function"
        private User NUSNetUser()
        {
            try
            {
                User u = (User)Session["nusNETuser"];
                return u;
            }
            catch (Exception ex)
            {
            }
            return null;
        }

        private int EventID()
        {
            int eventID = int.Parse(Request.QueryString["EventID"]);
            return eventID;
        }

        private string workSpaceDir(string folderName = "-")
        {
            if (folderName == "-")
                folderName = "";

            return Server.MapPath("~") + wrkSpaceDir + EventID().ToString() + "\\" + folderName;
        }
        #endregion

        private string UploadFile(string fileName)
        {
            string uploadPath = workSpaceDir(lblSelectedFolder.Text.Trim()) + "\\" + fileName;
            fuFileUpload.SaveAs(uploadPath);
            return uploadPath;
        }

        protected string Uploader(string s)
        {
            AdministrationClient adClient = new AdministrationClient();
            string uploader = adClient.GetUserName(s);
            adClient.Close();
            return uploader;
        }

        protected string hyperLinkage(string url, string fileName)
        {
            if (url != "")
                return url;

            string path = Request.Url.ToString().Replace(Request.RawUrl.Replace("%2f", "/"), "") + "/" + wrkSpaceDir.Replace("\\", "/") + EventID() + "/" + lblSelectedFolder.Text.Trim() + "/" + fileName;
            return path;
        }
    }
}