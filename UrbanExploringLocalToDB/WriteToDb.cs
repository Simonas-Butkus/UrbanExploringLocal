using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Data.SqlClient;
using System.Data;

namespace UrbanExploringLocalToDB
{
    
    class WriteToDb
    {
        public WriteToDb(ObjectInfo obj)
        {
            Write(obj);
        }

        private void Write(ObjectInfo obj)
        {
            string connectionString = "Data Source=SQL6002.site4now.net;Initial Catalog=DB_A2F009_NoLuck;User Id=DB_A2F009_NoLuck_admin;Password=Nikodemas1;";

            using (SqlConnection con = new SqlConnection())
            {
                con.ConnectionString = connectionString;
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.CommandText = "Insert Into Manor (Name, MapsURL, Info, Notes, MapsPhoto, SatellitePhoto, TopoPhoto) " +
                        "Values(@Name, @MapsURL, @Info, @Notes, @MapsPhoto, @SatellitePhoto, @TopoPhoto)";
                    cmd.Connection = con;
                    con.Open();

                    Debug.WriteLine(obj.Name);
                    cmd.Parameters.AddWithValue("@Name", obj.Name);
                    cmd.Parameters.AddWithValue("@MapsURL", obj.MapsURL);
                    cmd.Parameters.AddWithValue("@Info", obj.Info);
                    cmd.Parameters.AddWithValue("@Notes", obj.Notes);
                    cmd.Parameters.AddWithValue("@MapsPhoto", obj.MapsPhoto);
                    cmd.Parameters.AddWithValue("@SatellitePhoto", obj.SatellitePhoto);
                    cmd.Parameters.AddWithValue("@TopoPhoto", obj.TopoPhoto);

                    cmd.ExecuteNonQuery();

                    cmd.CommandText = "SELECT TOP 1 * FROM Manor ORDER BY Manor_Id DESC";
                    var Id = cmd.ExecuteScalar();

                    cmd.CommandText = "Insert Into Photos (Photo, Manor_Id) Values (@Photo, @Manor_Id)";
                    cmd.Parameters.Add("@Photo", SqlDbType.VarChar);
                    cmd.Parameters.Add("@Manor_Id", SqlDbType.Int);

                    if (obj.Photos != null)
                    {
                        foreach (var photo in obj.Photos)
                        {
                            if (photo != "" && photo != null)
                            {
                                cmd.Parameters["@Photo"].Value = photo;
                                cmd.Parameters["@Manor_Id"].Value = Id;

                                cmd.ExecuteNonQuery();
                            }
                        }
                    }

                    cmd.CommandText = "Insert Into Streetview (Streetview, Manor_Id) Values (@Streetview, @Manor_Id)";
                    cmd.Parameters.Add("@Streetview", SqlDbType.VarChar);

                    if (obj.Streetview != null)
                    {
                        foreach (var photo in obj.Streetview)
                        {
                            if (photo != "" && photo != null)
                            {

                                cmd.Parameters["@Streetview"].Value = photo;
                                cmd.Parameters["@Manor_Id"].Value = Id;

                                cmd.ExecuteNonQuery();
                            }
                        }
                    }

                    con.Close();
                }
            }
        }

    }


}
