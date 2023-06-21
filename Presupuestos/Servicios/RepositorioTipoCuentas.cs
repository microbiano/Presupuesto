using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Presupuestos.Models;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Presupuestos.Servicios
{
    public interface IRepositorioTipoCuentas
    {
        Task Actualizar(TipoCuenta tipoCuenta);
        Task Borrar(int id);
        Task Crear(TipoCuenta tipoCuenta);
        Task<bool> Existe(string nombre, int usuarioId);
        Task<IEnumerable<TipoCuenta>> Obtener(int usuarioId);
        Task<TipoCuenta> ObtenerPorId(int id, int usuarioId);
        Task Ordenar(IEnumerable<TipoCuenta> tipoCuentas);
    }
    public class RepositorioTipoCuentas : IRepositorioTipoCuentas
    {
        private readonly string ConnectionString;

        public RepositorioTipoCuentas(IConfiguration configuration)
        {
            ConnectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task Crear(TipoCuenta tipoCuenta) 
        { 
            using var connection =  new SqlConnection(ConnectionString);
            var id = await connection.QuerySingleAsync<int>("sp_TipoCuentas_Insertar", 
                                                            new {UsuarioId=tipoCuenta.UsuarioId,
                                                                 Nombre=tipoCuenta.Nombre},
                                                                commandType:CommandType.StoredProcedure);
            tipoCuenta.Id = id;

        }

        public async Task<bool> Existe(string nombre,int usuarioId)
        {
            using var connection = new SqlConnection(ConnectionString);
            var existe = await connection.QueryFirstOrDefaultAsync<int>(
                                        @"select 1 from TipoCuenta
                                        where Nombre=@Nombre and UsuarioId=@UsuarioId;",
                                        new { nombre, usuarioId });
            return existe == 1;
        }

        public async Task<IEnumerable<TipoCuenta>> Obtener(int usuarioId) 
        {
            using var connection = new SqlConnection(ConnectionString);
            return await connection.QueryAsync<TipoCuenta>(
                                        @"select Id,Nombre,UsuarioId,Orden from TipoCuenta
                                        where UsuarioId=@usuarioId
                                        Order by Orden",new { usuarioId});

        }

        public async Task Actualizar(TipoCuenta tipoCuenta)
        {
            using var connection = new SqlConnection(ConnectionString);
            await connection.ExecuteAsync(@"update TipoCuenta set Nombre=@Nombre
                                           where Id = @Id", tipoCuenta);
        }

        public async Task<TipoCuenta> ObtenerPorId(int id, int usuarioId)
        {
            using var connection = new SqlConnection(ConnectionString);
            return await connection.QueryFirstOrDefaultAsync<TipoCuenta>(@"Select id,nombre,usuarioid,orden from tipocuenta
                                                                           where id=@id and usuarioid=@usuarioid", new { id, usuarioId });
        }

        public async Task Borrar(int id)
        {
            using var connection = new SqlConnection(ConnectionString);
            await connection.ExecuteAsync("Delete from tipocuenta where id=@id",new { id});
        }

        public async Task Ordenar(IEnumerable<TipoCuenta> tipoCuentas)
        {
            var query = "update tipocuenta set orden=@Orden where Id=@Id;";
            using var connection = new SqlConnection(ConnectionString);
            await connection.ExecuteAsync(query,tipoCuentas);
        }
    }

}
