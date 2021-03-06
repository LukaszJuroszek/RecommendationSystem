﻿using RecomendationModel.Entities;
using RecomendationModel.ViewModel;
using System.Collections.Generic;
using System.Windows.Forms;

namespace WinRecomendationSystem
{
    public partial class FrmMain : Form
    {
        private MainViewModel _mainViewModel;
        private bool allOrRecomended = true;
        public FrmMain()
        {
            _mainViewModel = new MainViewModel();
            InitializeComponent();
        }
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            const string message = "Are you sure that you would like to exit ?";
            const string caption = "Cancel exit";
            var result = MessageBox.Show(message,caption,MessageBoxButtons.YesNo,MessageBoxIcon.Question);
            e.Cancel = ( result == DialogResult.No );
        }
        private void FrmMain_Load(object sender,System.EventArgs e)
        {
            ViewEvents(_mainViewModel.TicketEvents);
        }
        private void SetAutoAdjustColumnSize()
        {
            for (int i = 0;i < listView.Columns.Count;i++)
            {
                listView.Columns[i].Width = -2;
            }
        }
        private void ViewEvents(IEnumerable<TicketEvent> tickets)
        {
            RemoveFromListViewAllTicketEvents();
            foreach (var item in tickets)
            {
                var lv = new ListViewItem() { Text = item.Title.ToString() };
                lv.Tag = item.Id;
                lv.SubItems.Add(item.EventCategory.ToString());
                lv.SubItems.Add(item.Localization.ToString());
                lv.SubItems.Add(item.Date.ToShortDateString());
                listView.Items.Add(lv);
            }
            SetAutoAdjustColumnSize();
        }
        private void btnShow_Click(object sender,System.EventArgs e)
        {
            if (listView.SelectedItems.Count > 0)
            {
                var tickedEvent = _mainViewModel.GetTicketEventById(int.Parse(listView.SelectedItems[0].Tag.ToString()));
                var showTicketViewModel = new ShowTicketViewModel()
                {
                    TicketEvent = tickedEvent,
                    User = _mainViewModel.User
                };
                var frmShowTicket = new FrmShowTicket(showTicketViewModel);
                if (frmShowTicket.ShowDialog() == DialogResult.Cancel)
                {
                    _mainViewModel.AddClikedEvent(frmShowTicket._showClickedTicketViewModel);
                    _mainViewModel.AddOpinion(frmShowTicket._opinionViewModel);
                }
            }
            else
                MessageBox.Show("Pleas Select event and then click ShowTicket","Error while selecting Ticket Event");
        }
        private void bntShowRecomendation_Click(object sender,System.EventArgs e)
        {
            if (allOrRecomended)
            {
                ViewEvents(_mainViewModel.GetRemommendedTicketEvents());
                textBox1.Text = _mainViewModel.GetRecomendationString();
                allOrRecomended = false;
            }
            else
            {
                ViewEvents(_mainViewModel.TicketEvents);
                allOrRecomended = true;
            }
        }
        private void RemoveFromListViewAllTicketEvents()
        {
            foreach (ListViewItem item in listView.Items)
            {
                item.Remove();
            }
        }
    }
}
