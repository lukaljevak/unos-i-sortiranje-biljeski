using System;
using System.Windows.Forms;

namespace NoteSort
{
    public partial class upisBiljeske : Form
    {
        private Notes mainForm;
        private int editIndex = -1;

        public upisBiljeske(Notes form)
        {
            InitializeComponent();
            mainForm = form;
        }

        public upisBiljeske(Notes form, string naslov, string opis, int index)
        {
            InitializeComponent();
            mainForm = form;
            txtTitle.Text = naslov;
            txtDescription.Text = opis;
            editIndex = index;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtTitle.Text) || string.IsNullOrWhiteSpace(txtDescription.Text))
            {
                MessageBox.Show("Naslov i opis ne mogu biti prazni.");
                return;
            }

            if (editIndex == -1)
                mainForm.AddNote(txtTitle.Text, txtDescription.Text);
            else
                mainForm.UpdateNote(editIndex, txtTitle.Text, txtDescription.Text);

            this.Close();
        }
    }
}