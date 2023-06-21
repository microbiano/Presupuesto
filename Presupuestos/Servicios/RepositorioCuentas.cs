using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Presupuestos.Models;
using Presupuestos.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Presupuestos.Servicios
{
    public interface IRepositorioCuentas
    {
        Task Actualizar(CuentaCreacionViewModel cuenta);
        Task Borrar(int id);
        Task<IEnumerable<Cuenta>> Buscar(int usuarioId);
        Task Crear(Cuenta cuenta);
        Task<Cuenta> ObtenerPorId(int id, int usuarioId);
    }
    public class RepositorioCuentas:IRepositorioCuentas
    {
        private readonly string ConnectionString;

        public RepositorioCuentas(IConfiguration configuration)
        {
            ConnectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task Crear(Cuenta cuenta)
        {
            using var connection = new SqlConnection(ConnectionString);
            var id = await connection.QuerySingleAsync<int>(@"insert into Cuenta(Nombre,TipoCuentaId,Balance,Descripcion) 
                                                            values (@Nombre,@TipoCuentaId,@Balance,@Descripcion);
                                                            select SCOPE_IDENTITY();",cuenta);
            cuenta.Id = id;
        }

        public async Task<IEnumerable<Cuenta>> Buscar(int usuarioId)
        {
            using var connection = new SqlConnection(ConnectionString);
            return await connection.QueryAsync<Cuenta>(@"select Cuenta.id,Cuenta.Nombre,Cuenta.Balance, tc.Nombre as TipoCuenta
                                                        from Cuenta 
                                                        inner join TipoCuenta tc on tc.Id= Cuenta.TipoCuentaId
                                                        where UsuarioId=@usuarioId
                                                        order by tc.Orden",new {usuarioId});

        }

        public async Task<Cuenta> ObtenerPorId(int id, int usuarioId)
        {
            using var connection = new SqlConnection(ConnectionString);
            return await connection.QueryFirstOrDefaultAsync<Cuenta>(
                                                    @"select Cuenta.id,Cuenta.Nombre,Cuenta.Balance,Descripcion,tc.id 
                                                    from Cuenta
                                                    inner join TipoCuenta tc on tc.Id = Cuenta.TipoCuentaId
                                                    where tc.UsuarioId = @usuarioId and cuenta.Id=@Id", new {id,usuarioId});
        }

        public async Task Actualizar(CuentaCreacionViewModel cuenta)
        {
            using var connection = new SqlConnection(ConnectionString);
            await connection.ExecuteAsync(
                                            @"update cuenta set
                                                nombre=@Nombre,Balance=@Balance,Descripcion=@Descripcion,TipoCuentaId=@TipoCuentaId
                                                where id=@Id", cuenta);
        }


        public async Task Borrar(int id)
        {
            using var connection = new SqlConnection(ConnectionString);
            await connection.ExecuteAsync(
                                            @"delete from cuenta where id=@Id", new { id});
        }
    }
}
