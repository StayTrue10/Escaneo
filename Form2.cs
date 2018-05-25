using SAPbobsCOM;
using System;
using System.Windows.Forms;

namespace Scaneo
{
    public partial class Form2 : Form
    {
        Recordset oRecordSet;
        public Form2()
        {
            InitializeComponent();
            ClaseCompartida.company.Server = "10.10.50.201"; //Servidor en sql
            ClaseCompartida.company.SLDServer = "10.10.50.201:40000"; //Servidor diapi + puerto
            ClaseCompartida.company.DbUserName = "sa"; //usuario sql
            ClaseCompartida.company.DbPassword = "Pr0c3s0.12"; // contraseña sql
            ClaseCompartida.company.DbServerType = BoDataServerTypes.dst_MSSQL2014; //tipo de servidor
            oRecordSet = ClaseCompartida.company.GetCompanyList();
            while (!(oRecordSet.EoF == true))
            {
                //// add the value of the first field of the Recordset
                cbBases.Items.Add(oRecordSet.Fields.Item(0).Value);
                //// move the record pointer to the next row
                oRecordSet.MoveNext();
            }
        }
        void btnIniciar_Click(object sender, EventArgs e)
        {
            try
            {
                ClaseCompartida.company.CompanyDB = cbBases.Text; //Nombre de la base de datos
                ClaseCompartida.company.UserName = tbUsuario.Text; //usuario root sap
                ClaseCompartida.company.Password = tbContraseña.Text; // contraseña root sap
                ClaseCompartida.ret = ClaseCompartida.company.Connect();
                string errMsg = ClaseCompartida.company.GetLastErrorDescription();
                if (ClaseCompartida.ret == 0)
                {
                    MessageBox.Show("Conectado a: " + ClaseCompartida.company.CompanyName + " en la base de datos: " + ClaseCompartida.company.CompanyDB);
                    Form1 formaSiguiente = new Form1();
                    Hide(); //oculta la forma actual
                    formaSiguiente.Show(); // muestra la forma2
                }
                else
                    MessageBox.Show(errMsg);
            }
            catch (Exception errMsgh)
            {
                throw errMsgh;
            }
        }
    }
}
