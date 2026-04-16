using Itv.Enums;
using Itv.Models;

namespace Itv.Factory;
/// <summary>
/// Clase con el metodo Seed() con los datos por defecto de carga.
/// </summary>
public static class VehiculoFactory {

    public static IEnumerable<Vehiculo> Seed() {

        var lista = new List<Vehiculo> {
            new Vehiculo {
                Matricula = "1234ABC", Marca = "Toyota", Modelo = "Corolla", Cilindrada = 1800,
                Motor = Motor.Hibrido, DniDueño = "12345678A", CreateAt = DateTime.Now, UpdateAt = DateTime.Now,
                IsDelete = false
            },
            new Vehiculo {
                Matricula = "5678DEF", Marca = "Volkswagen", Modelo = "Golf", Cilindrada = 2000,
                Motor = Motor.Diesel, DniDueño = "23456789B", CreateAt = DateTime.Now, UpdateAt = DateTime.Now,
                IsDelete = false
            },
            new Vehiculo {
                Matricula = "9012GHI", Marca = "Tesla", Modelo = "Model 3", Cilindrada = 0,
                Motor = Motor.Electrico, DniDueño = "34567890C", CreateAt = DateTime.Now, UpdateAt = DateTime.Now,
                IsDelete = false
            },
            new Vehiculo {
                Matricula = "3456JKL", Marca = "Ford", Modelo = "Focus", Cilindrada = 1500,
                Motor = Motor.Gasolina, DniDueño = "45678901D", CreateAt = DateTime.Now, UpdateAt = DateTime.Now,
                IsDelete = false
            },
            new Vehiculo {
                Matricula = "7890MNO", Marca = "Hyundai", Modelo = "Ioniq", Cilindrada = 1600,
                Motor = Motor.Hibrido, DniDueño = "56789012E", CreateAt = DateTime.Now, UpdateAt = DateTime.Now,
                IsDelete = false
            },
            new Vehiculo {
                Matricula = "1122PQR", Marca = "BMW", Modelo = "320d", Cilindrada = 2000, Motor = Motor.Diesel,
                DniDueño = "67890123F", CreateAt = DateTime.Now, UpdateAt = DateTime.Now, IsDelete = false
            },
            new Vehiculo {
                Matricula = "3344STU", Marca = "Renault", Modelo = "Zoe", Cilindrada = 0,
                Motor = Motor.Electrico, DniDueño = "78901234G", CreateAt = DateTime.Now, UpdateAt = DateTime.Now,
                IsDelete = true
            },
            new Vehiculo {
                Matricula = "5566VWX", Marca = "Seat", Modelo = "Ibiza", Cilindrada = 1000,
                Motor = Motor.Gasolina, DniDueño = "89012345H", CreateAt = DateTime.Now, UpdateAt = DateTime.Now,
                IsDelete = false
            },
            new Vehiculo {
                Matricula = "7788YZA", Marca = "Mercedes", Modelo = "Class A", Cilindrada = 2200,
                Motor = Motor.Diesel, DniDueño = "90123456I", CreateAt = DateTime.Now, UpdateAt = DateTime.Now,
                IsDelete = false
            },
            new Vehiculo {
                Matricula = "9900BCD", Marca = "Kia", Modelo = "Niro", Cilindrada = 1600,
                Motor = Motor.Hibrido, DniDueño = "01234567J", CreateAt = DateTime.Now, UpdateAt = DateTime.Now,
                IsDelete = false
            }
        };

        return lista;
    }
}