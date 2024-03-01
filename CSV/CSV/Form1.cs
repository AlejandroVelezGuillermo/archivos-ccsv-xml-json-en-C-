using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace CSV
{
    public partial class Form1 : Form
    {
        // Lista para almacenar los datos
        private List<registros> registros = new List<registros>();
        // Ruta del archivo CSV actualmente abierto
        private string rutaArchivoActual = "";

        string formato;
        public Form1()
        {
            InitializeComponent();
            // Establecer la propiedad DropDownStyle del ComboBox
            comboBox1.DropDownStyle = ComboBoxStyle.DropDownList;
        }
        private void aGREGARToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Validar que los campos no estén vacíos
            if (string.IsNullOrWhiteSpace(txtNombre.Text) || string.IsNullOrWhiteSpace(txtTelefono.Text) || string.IsNullOrWhiteSpace(txtCorreo.Text))
            {
                MessageBox.Show("Por favor, complete todos los campos antes de agregar.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            // Crear un nuevo registro
            registros nuevoRegistro = new registros
            {
                Nombre = txtNombre.Text,
                Telefono = txtTelefono.Text,
                Correo = txtCorreo.Text
            };

            // Agregar el registro a la lista y al DataGridView
            registros.Add(nuevoRegistro);
            dgvDatos.DataSource = null; // Limpiar el origen de datos actual
            dgvDatos.DataSource = registros; // Asignar la nueva lista de registros
            LimpiarCampos();
        }
        private void gUARDARToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string NombreA = textBox1.Text;
            try
            {
                // Obtener la ruta del escritorio del usuario actual
                string escritorio = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

                // Crear la ruta completa del archivo CSV en el escritorio
                string rutaArchivo = Path.Combine(escritorio, NombreA + "." + formato);

                // Crear y escribir en el archivo CSV
                using (StreamWriter writer = new StreamWriter(rutaArchivo))
                {
                    // Escribir encabezados
                    writer.WriteLine("Nombre,Telefono,Correo");

                    // Escribir datos
                    foreach (registros registro in registros)
                    {
                        writer.WriteLine($"{registro.Nombre},{registro.Telefono},{registro.Correo}");
                    }
                }

                MessageBox.Show($"Datos guardados exitosamente en el archivo CSV en el escritorio ({rutaArchivo}).", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LimpiarCampos();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al guardar en el archivo CSV: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            // Limpiar el DataGridView
            dgvDatos.DataSource = null;
            dgvDatos.Rows.Clear(); // Esto debería funcionar, pero si hay problemas, asegúrate de que el DataGridView está configurado correctamente

            // Limpiar la lista de registros
            registros.Clear();
        }
        private void LimpiarCampos()
        {
            // Limpiar los campos de entrada
            txtNombre.Text = "";
            txtTelefono.Text = "";
            txtCorreo.Text = "";
            textBox1.Text = "";
        }
        private void aBRIRToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Diálogo para abrir el archivo CSV
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Archivos CSV |*.csv |Archivos Txt|*.txt|Archivos xml|*.xml|Archivos json|*.json|Todos Los Archivos|*.*",
                Title = "Archivos Cargados Correctamente."
            };


            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                // Obtener la ruta del archivo seleccionado
                string rutaArchivo = openFileDialog.FileName;

                // Leer datos desde el archivo CSV
                using (StreamReader reader = new StreamReader(rutaArchivo))
                {
                    // Saltar la primera línea (encabezados)
                    reader.ReadLine();

                    // Limpiar la lista actual de registros
                    registros.Clear();

                    // Leer y agregar registros desde el archivo
                    while (!reader.EndOfStream)
                    {
                        string[] campos = reader.ReadLine().Split(',');
                        registros nuevoRegistro = new registros
                        {
                            Nombre = campos[0],
                            Telefono = campos[1],
                            Correo = campos[2]
                        };
                        registros.Add(nuevoRegistro);
                    }
                }

                // Mostrar los datos en el DataGridView
                dgvDatos.DataSource = null;
                dgvDatos.DataSource = registros;

                MessageBox.Show("Datos cargados exitosamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        private void rEMPLACARToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Obtener el valor seleccionado del ComboBox
            object valorSeleccionado = comboBox1.SelectedItem;

            // Si es una cadena, puedes convertirlo a string si es necesario
            if (valorSeleccionado != null)
            {
               formato = valorSeleccionado.ToString();
            }
        }
        private void eDITARToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Verificar si hay al menos un renglón seleccionado
            if (dgvDatos.SelectedRows.Count > 0)
            {
                // Obtener el índice del primer renglón seleccionado
                int indiceSeleccionado = dgvDatos.SelectedRows[0].Index;

                // Obtener la fila completa del renglón seleccionado
                DataGridViewRow filaSeleccionada = dgvDatos.Rows[indiceSeleccionado];

                // Obtener los valores de todas las celdas del renglón
                string valorCelda0 = filaSeleccionada.Cells[0].Value.ToString();
                string valorCelda1 = filaSeleccionada.Cells[1].Value.ToString();
                string valorCelda2 = filaSeleccionada.Cells[2].Value.ToString();

                // Mostrar los valores en los TextBox
                txtNombre.Text = valorCelda0;
                txtTelefono.Text = valorCelda1;
                txtCorreo.Text = valorCelda2;
            }
        }

        private void gUARDADCOMToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Obtén la información del cuadro de texto
            string datos = formato;

            // Verifica si el cuadro de texto no está vacío
            if (!string.IsNullOrEmpty(datos))
            {
                // Configura el cuadro de diálogo "Guardar como"
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "Archivos de Texto|*.txt|Todos los archivos|*.*";
                saveFileDialog.Title = "Guardar Como";
                saveFileDialog.FileName = textBox1.Text;

                // Muestra el cuadro de diálogo
                DialogResult result = saveFileDialog.ShowDialog();

                // Si el usuario hace clic en "Guardar"
                if (result == DialogResult.OK)
                {
                    try
                    {
                        // Obtiene la ruta del archivo seleccionada por el usuario
                        string rutaArchivo = saveFileDialog.FileName;

                        // Guarda los datos en el archivo
                        File.WriteAllText(rutaArchivo, datos);
                        MessageBox.Show("Datos guardados correctamente", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error al guardar datos: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Por favor, ingresa datos antes de guardar", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}
