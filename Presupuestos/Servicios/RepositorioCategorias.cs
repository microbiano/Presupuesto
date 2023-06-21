using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Presupuestos.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Presupuestos.Servicios
{
    public interface IRepositorioCategorias
    {
        Task Crear(Categoria categoria);
        Task<IEnumerable<Categoria>> Obtener(int usuarioId);
    }
    public class RepositorioCategorias : IRepositorioCategorias
    {
        private readonly string ConnectionString;
        public RepositorioCategorias(IConfiguration configuration)
        {
            ConnectionString = configuration.GetConnectionString("DefaultConnection");

        }

        public async Task Crear(Categoria categoria)
        {
            using var connection = new SqlConnection(ConnectionString);

            var id = await connection.QuerySingleAsync<int>(@"insert into Categoria(Nombre,TipoOperacionId,UsuarioId) 
                                                            values (@Nombre,@TipoOperacionId,@UsuarioId);
                                                            select SCOPE_IDENTITY();", categoria);
            categoria.Id = id;
        }

        public async Task<IEnumerable<Categoria>> Obtener(int usuarioId)
        {
            using var connection = new SqlConnection(ConnectionString);
            return await connection.QueryAsync<Categoria>(
                                    "Select * from  Categoria where UsuarioId=@UsuarioId", new { usuarioId });
        }
    }
}
