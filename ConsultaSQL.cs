using System.Data.SqlClient;

namespace Scaneo
{
    class ConsultaSQL
    {
        //Conexion a sql server
        SqlConnection conexion = new SqlConnection("Data Source = 10.10.50.201; Initial Catalog = RUTACRITICA; user id=sa; password=Pr0c3s0.12");
        //Insert datos a bd (varchar, int, date, time)
        public bool Contador(int id, int orden)
        {
            //Abre la conexion e inserta en las columas correspondientes
            conexion.Open();
            SqlCommand cmd = new SqlCommand(string.Format("Insert into ContadorDIAPI values ({0}, {1})", id, orden), conexion);
            //Filas afectadas mayores a 0, registradas con exito
            int n = cmd.ExecuteNonQuery();
            conexion.Close();
            if (n > 0)
                return true;
            return false;
        }
    }
}
