using SAPbobsCOM;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace Scaneo
{
    public partial class Form1 : Form
    {
        ConsultaSQL sql = new ConsultaSQL();
        SqlConnection conexion = new SqlConnection("Data Source = 10.10.50.201; Initial Catalog = '" + ClaseCompartida.company.CompanyDB + "'; user id=sa; password=Pr0c3s0.12");
        SqlConnection cn = new SqlConnection("Data Source = 10.10.50.201; Initial Catalog = RUTACRITICA; user id=sa; password=Pr0c3s0.12");
        DataTable orden = new DataTable(); //Datos orden de compra
        DataTable codigos = new DataTable(); //Datos itemname etc
        DataTable final = new DataTable(); //vista al grid
        int n = 0;
        int miss = 0;
        int err = 0;
        int fila = 0;
        int completo;
        string faltante;
        public Form1()
        {
            InitializeComponent();
            final.Columns.Add("CodeBars", typeof(string));
            final.Columns.Add("ItemCode", typeof(string));
            final.Columns.Add("ItemName", typeof(string));
            final.Columns.Add("Cantidad", typeof(Int32));
        }
        void btnBuscar_Click(object sender, EventArgs e)
        {
            final.Clear();
            orden.Clear();
            if (int.TryParse(tbOrdencompra.Text, out n))
            {
                SqlDataAdapter datos = new SqlDataAdapter("Select codebars,itemcode,quantity,docentry from DRF1 where linestatus = 'o' and docentry = '" + tbOrdencompra.Text + "'", conexion);
                datos.Fill(orden);
                dataGridView1.DataSource = orden;
                if (orden.Rows.Count == 0)
                    MessageBox.Show("No se encuentra la orden de compra", "Scaneo", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            else
                MessageBox.Show("Ingrese un numero", "Scaneo", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        }
        void btnLimpiar_Click(object sender, EventArgs e)
        {
            tbCodebars.Text = "";
            tbOrdencompra.Text = "";
            dataGridView1.DataSource = null;
        }
        void btnIngresar_Click(object sender, EventArgs e)
        {
            codigos.Clear();
            if (tbCodebars.TextLength > 0)
            {
                if (orden.Rows.Count > 0) // existe orden de compra
                {
                    for (int i = 0; i < orden.Rows.Count; i++)
                    {
                        if (tbCodebars.Text == orden.Rows[i]["codebars"].ToString() || tbCodebars.Text == orden.Rows[i]["itemcode"].ToString())
                        {
                            err = 1;
                            fila = i;
                        }
                    }
                    if (err == 0)
                        MessageBox.Show("No se encontro este producto en la orden ingresada", "Scaneo", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    else if (err == 1)
                    {
                        err = 0;
                        SqlDataAdapter datos = new SqlDataAdapter("Select codebars, itemcode, itemname from OITM where codebars = '" + tbCodebars.Text + "' or itemcode = '" + tbCodebars.Text + "'", conexion);
                        datos.Fill(codigos);
                        if (codigos.Rows.Count > 0)
                        {
                            if (0 < Convert.ToInt32(orden.Rows[fila]["quantity"])) //que no se pase de la cantidad en la orden
                            {
                                final.Rows.Add(codigos.Rows[0][0], codigos.Rows[0][1], codigos.Rows[0][2], 1);
                                orden.Rows[fila]["quantity"] = Convert.ToInt32(orden.Rows[fila]["quantity"]) - 1;
                                dataGridView1.DataSource = final;
                            }
                            else
                                MessageBox.Show("No se puede exceder la cantidad de este producto", "Scaneo", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        }
                        else
                            MessageBox.Show("Este producto no existe en la base de datos", "Scaneo", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                }
            }
            else
                MessageBox.Show("Ingrese el codigo de barras", "Scaneo", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            tbCodebars.Text = "";
        }
        void tbCodebars_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                codigos.Clear();
                if (tbCodebars.TextLength > 0)
                {
                    if (orden.Rows.Count > 0) // existe orden de compra
                    {
                        for (int i = 0; i < orden.Rows.Count; i++)
                        {
                            if (tbCodebars.Text == orden.Rows[i]["codebars"].ToString() || tbCodebars.Text == orden.Rows[i]["itemcode"].ToString())
                            {
                                err = 1;
                                fila = i;
                            }
                        }
                        if (err == 0)
                            MessageBox.Show("No se encontro este producto en la orden ingresada", "Scaneo", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        else if (err == 1)
                        {
                            err = 0;
                            SqlDataAdapter datos = new SqlDataAdapter("Select codebars, itemcode, itemname from OITM where codebars = '" + tbCodebars.Text + "' or itemcode = '" + tbCodebars.Text + "'", conexion);
                            datos.Fill(codigos);
                            if (codigos.Rows.Count > 0)
                            {
                                if (0 < Convert.ToInt32(orden.Rows[fila]["quantity"])) //que no se pase de la cantidad en la orden
                                {
                                    final.Rows.Add(codigos.Rows[0][0], codigos.Rows[0][1], codigos.Rows[0][2], 1);
                                    orden.Rows[fila]["quantity"] = Convert.ToInt32(orden.Rows[fila]["quantity"]) - 1;
                                    dataGridView1.DataSource = final;
                                }
                                else
                                    MessageBox.Show("No se puede exceder la cantidad de este producto", "Scaneo", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            }
                            else
                                MessageBox.Show("Este producto no existe en la base de datos", "Scaneo", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        }
                    }
                }
                else
                    MessageBox.Show("Ingrese primero la orden de compra", "Scaneo", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                tbCodebars.Text = "";
            }
        }
        void btnCrear_Click(object sender, EventArgs e)
        {
            completo = 0;
            for (int i = 0; i < orden.Rows.Count; i++)
            {
                if (0 < Convert.ToInt32(orden.Rows[i]["quantity"]))
                {
                    miss = i;
                    faltante = orden.Rows[miss]["itemcode"].ToString();
                    if (MessageBox.Show("Orden incompleta falta " + faltante + " ¿Desea continuar con el registro?", "Scaneo", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                    {
                        orden.Rows[miss]["quantity"] = 0;
                    }
                }
                completo = completo + Convert.ToInt32(orden.Rows[i]["quantity"]);
            }
            if (completo == 0)
            {
                if (tbOrdencompra.Text == orden.Rows[0]["docentry"].ToString())
                {
                    DataTable diapi = new DataTable();
                    SqlDataAdapter ordenc = new SqlDataAdapter("Select Orden from ContadorDIAPI WHERE Orden = '" + tbOrdencompra.Text + "'", cn);
                    ordenc.Fill(diapi);
                    if (diapi.Rows.Count == 1)
                        MessageBox.Show("La orden de compra ya esta registrada", "Scaneo", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    else
                    {
                        DataTable contador = new DataTable();
                        int conta;
                        SqlDataAdapter datos = new SqlDataAdapter("Select * from ContadorDIAPI", cn);
                        datos.Fill(contador);
                        conta = 100 + contador.Rows.Count;
                        CompanyService companyservice = ClaseCompartida.company.GetCompanyService();
                        GeneralService generalservice = companyservice.GetGeneralService("SCAN");
                        GeneralDataParams dataparams = null;
                        dataparams = (GeneralDataParams)generalservice.GetDataInterface(GeneralServiceDataInterfaces.gsGeneralDataParams);
                        //Create data for new row in main UDO
                        GeneralData DATA = (GeneralData)generalservice.GetDataInterface(GeneralServiceDataInterfaces.gsGeneralData);
                        GeneralDataCollection children = DATA.Child("SCAN_DET");
                        // Add
                        DATA.SetProperty("Code", conta.ToString());
                        DATA.SetProperty("U_ALM", tbOrdencompra.Text);
                        DATA.SetProperty("U_Fecha", DateTime.Now);
                        for (int i = 0; i < final.Rows.Count; i++)
                        {
                            GeneralData child = children.Add();
                            child.SetProperty("U_CBB", final.Rows[i][0].ToString());
                            child.SetProperty("U_QTY", final.Rows[i][3].ToString());
                            child.SetProperty("U_ITEMCODE", final.Rows[i][1].ToString());
                        }
                        generalservice.Add(DATA);
                        sql.Contador(conta, Convert.ToInt32(tbOrdencompra.Text));
                        MessageBox.Show("Producto Registrado", "Scaneo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        dataGridView1.DataSource = null;
                        tbOrdencompra.Text = "";
                    }
                }
                else
                    MessageBox.Show("Los productos no pertenecen a esta orden", "Scaneo", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            //Update
            //oUDO_Name.SetProperty("Code", sCode);
            //oUDO_Name.SetProperty("Name", sName);
            //oUDO_Name.SetProperty("U_Remark", sRemark);
            //oUDO_Name.SetProperty("U_ActiveYN", sActiveYN);
            //generalservice.Update(oUDO_Name);
            ////Delete
            //dataparams.SetProperty("Code", sCode);
            //generalservice.Delete(dataparams);
        }

        void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            int total = 0;

            int escaneado = 0;
            DataGridViewRow fila = dataGridView1.Rows[e.RowIndex];
            for (int i = 0; i < orden.Rows.Count; i++)
            {
                if (Convert.ToString(fila.Cells[1].Value) == orden.Rows[i]["itemcode"].ToString())
                {
                    tbCantidadfal.Text = orden.Rows[i]["quantity"].ToString();
                }
            }
            for (int i = 0; i < final.Rows.Count; i++)
            {
                if (Convert.ToString(fila.Cells[1].Value) == final.Rows[i]["ItemCode"].ToString())
                {
                    escaneado++;
                }
            }
            tbCantidadesc.Text = escaneado.ToString();
            total = escaneado + Convert.ToInt32(tbCantidadfal.Text);
            tbCantidadorden.Text = total.ToString();
        }
    }
}