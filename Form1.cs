using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Windows.Forms;

namespace NoteSort
{
    public partial class Notes : Form
    {
        private DataTable notesList = new DataTable();
        private string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "NoteSort", "notes.json");

        public Notes()
        {
            InitializeComponent();
            LoadNotes();
        }

        private void Notes_Load(object sender, EventArgs e)
        {
            notesList.Columns.Add("Naslov");
            notesList.Columns.Add("Opis");
            dataGridView1.DataSource = notesList;
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            upisBiljeske upisForm = new upisBiljeske(this);
            upisForm.ShowDialog();
        }

        public void AddNote(string naslov, string opis)
        {
            notesList.Rows.Add(naslov, opis);
            SaveNotes();
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow != null)
            {
                string naslov = dataGridView1.CurrentRow.Cells[0].Value.ToString();
                string opis = dataGridView1.CurrentRow.Cells[1].Value.ToString();

                upisBiljeske upisForm = new upisBiljeske(this, naslov, opis, dataGridView1.CurrentRow.Index);
                upisForm.ShowDialog();
            }
        }

        public void UpdateNote(int index, string naslov, string opis)
        {
            notesList.Rows[index]["Naslov"] = naslov;
            notesList.Rows[index]["Opis"] = opis;
            SaveNotes();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow != null)
            {
                var result = MessageBox.Show("Jeste li sigurni da želite obrisati ovu bilješku?", "Potvrdi brisanje", MessageBoxButtons.YesNo);
                if (result == DialogResult.Yes)
                {
                    notesList.Rows.RemoveAt(dataGridView1.CurrentRow.Index);
                    SaveNotes();
                }
            }
        }

        private void SaveNotes()
        {
            try
            {
                var notes = notesList.AsEnumerable()
                    .Select(row => new Dictionary<string, string>
                    {
                        { "Naslov", row["Naslov"].ToString() },
                        { "Opis", row["Opis"].ToString() }
                    })
                    .ToList();

                Directory.CreateDirectory(Path.GetDirectoryName(filePath));
                File.WriteAllText(filePath, JsonSerializer.Serialize(notes));
            }
            catch (Exception ex)
            {
                MessageBox.Show("Greška pri spremanju bilješki: " + ex.Message);
            }
        }

        private void LoadNotes()
        {
            try
            {
                if (File.Exists(filePath))
                {
                    var notes = JsonSerializer.Deserialize<List<Dictionary<string, string>>>(File.ReadAllText(filePath));

                    if (notes != null)
                    {
                        foreach (var note in notes)
                        {
                            if (note.ContainsKey("Naslov") && note.ContainsKey("Opis"))
                            {
                                notesList.Rows.Add(note["Naslov"], note["Opis"]);
                            }
                            else
                            {
                                MessageBox.Show("Jedna ili više bilješki imaju neispravan format i neće biti učitane.");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Greška pri učitavanju bilješki: " + ex.Message);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            SaveNotes();
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            string filter = txtSearch.Text;
            notesList.DefaultView.RowFilter = $"Naslov LIKE '%{filter}%' OR Opis LIKE '%{filter}%'";
        }
    }
}