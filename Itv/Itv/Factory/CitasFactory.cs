using Itv.Enums;
using Itv.Models;

namespace Itv.Factory;
/// <summary>
/// Clase con el metodo Seed() con los datos por defecto de carga.
/// </summary>
public static class CitasFactory {
    
    public static IEnumerable<Cita> Seed() {
        return new List<Cita> {
            new Cita { Matricula = "1234BBB", Marca = "Toyota", Modelo = "Corolla", Cilindrada = 1800, Motor = Motor.Diesel, DniDueño = "12345678Z", FechaMatriculacion = new DateTime(2018, 5, 10), FechaInspeccion = DateTime.Today.AddDays(-15) },
            new Cita { Matricula = "2345BCD", Marca = "Audi", Modelo = "A3", Cilindrada = 2000, Motor = Motor.Gasolina, DniDueño = "87654321X", FechaMatriculacion = new DateTime(2020, 1, 15), FechaInspeccion = DateTime.Today.AddDays(-14) },
            new Cita { Matricula = "3456BCF", Marca = "Seat", Modelo = "Ibiza", Cilindrada = 1400, Motor = Motor.Gasolina, DniDueño = "11223344B", FechaMatriculacion = new DateTime(2017, 3, 22), FechaInspeccion = DateTime.Today.AddDays(-13) },
            new Cita { Matricula = "4567BCG", Marca = "Volkswagen", Modelo = "Golf", Cilindrada = 1600, Motor = Motor.Diesel, DniDueño = "44332211X", FechaMatriculacion = new DateTime(2019, 7, 5), FechaInspeccion = DateTime.Today.AddDays(-12) },
            new Cita { Matricula = "5678BCH", Marca = "Renault", Modelo = "Clio", Cilindrada = 1200, Motor = Motor.Gasolina, DniDueño = "23456789D", FechaMatriculacion = new DateTime(2016, 11, 18), FechaInspeccion = DateTime.Today.AddDays(-11) },
            new Cita { Matricula = "6789BCJ", Marca = "Peugeot", Modelo = "308", Cilindrada = 1500, Motor = Motor.Diesel, DniDueño = "34567890V", FechaMatriculacion = new DateTime(2021, 2, 9), FechaInspeccion = DateTime.Today.AddDays(-10) },
            new Cita { Matricula = "7890BCK", Marca = "Citroen", Modelo = "C4", Cilindrada = 1600, Motor = Motor.Diesel, DniDueño = "45678901G", FechaMatriculacion = new DateTime(2015, 6, 30), FechaInspeccion = DateTime.Today.AddDays(-9) },
            new Cita { Matricula = "8901BCL", Marca = "Ford", Modelo = "Focus", Cilindrada = 1800, Motor = Motor.Gasolina, DniDueño = "56789012B", FechaMatriculacion = new DateTime(2014, 9, 12), FechaInspeccion = DateTime.Today.AddDays(-8) },
            new Cita { Matricula = "9012BCM", Marca = "Opel", Modelo = "Astra", Cilindrada = 1700, Motor = Motor.Diesel, DniDueño = "67890123B", FechaMatriculacion = new DateTime(2013, 10, 25), FechaInspeccion = DateTime.Today.AddDays(-7) },
            new Cita { Matricula = "1122BCN", Marca = "BMW", Modelo = "Serie 1", Cilindrada = 2000, Motor = Motor.Diesel, DniDueño = "78901234X", FechaMatriculacion = new DateTime(2022, 4, 3), FechaInspeccion = DateTime.Today.AddDays(-6) },

            new Cita { Matricula = "2233BCP", Marca = "Mercedes", Modelo = "Clase A", Cilindrada = 1300, Motor = Motor.Gasolina, DniDueño = "89012345E", FechaMatriculacion = new DateTime(2021, 8, 14), FechaInspeccion = DateTime.Today.AddDays(-5) },
            new Cita { Matricula = "3344BCR", Marca = "Kia", Modelo = "Ceed", Cilindrada = 1600, Motor = Motor.Hibrido, DniDueño = "90123456A", FechaMatriculacion = new DateTime(2020, 12, 1), FechaInspeccion = DateTime.Today.AddDays(-4) },
            new Cita { Matricula = "4455BCS", Marca = "Hyundai", Modelo = "i30", Cilindrada = 1500, Motor = Motor.Gasolina, DniDueño = "10293847J", FechaMatriculacion = new DateTime(2018, 1, 20), FechaInspeccion = DateTime.Today.AddDays(-3) },
            new Cita { Matricula = "5566BCT", Marca = "Nissan", Modelo = "Qashqai", Cilindrada = 1600, Motor = Motor.Diesel, DniDueño = "56473829C", FechaMatriculacion = new DateTime(2017, 5, 17), FechaInspeccion = DateTime.Today.AddDays(-2) },
            new Cita { Matricula = "6677BCV", Marca = "Mazda", Modelo = "CX-30", Cilindrada = 2000, Motor = Motor.Gasolina, DniDueño = "91827364W", FechaMatriculacion = new DateTime(2022, 9, 29), FechaInspeccion = DateTime.Today.AddDays(-1) },
            new Cita { Matricula = "7788BCW", Marca = "Honda", Modelo = "Civic", Cilindrada = 1800, Motor = Motor.Hibrido, DniDueño = "18273645F", FechaMatriculacion = new DateTime(2019, 2, 11), FechaInspeccion = DateTime.Today },
            new Cita { Matricula = "8899BCX", Marca = "Skoda", Modelo = "Octavia", Cilindrada = 2000, Motor = Motor.Diesel, DniDueño = "83726154J", FechaMatriculacion = new DateTime(2016, 4, 8), FechaInspeccion = DateTime.Today.AddDays(1) },
            new Cita { Matricula = "9900BCY", Marca = "Fiat", Modelo = "Tipo", Cilindrada = 1400, Motor = Motor.Gasolina, DniDueño = "72635481D", FechaMatriculacion = new DateTime(2015, 12, 21), FechaInspeccion = DateTime.Today.AddDays(2) },
            new Cita { Matricula = "1010BCZ", Marca = "Dacia", Modelo = "Sandero", Cilindrada = 1000, Motor = Motor.Gasolina, DniDueño = "61524378E", FechaMatriculacion = new DateTime(2023, 1, 4), FechaInspeccion = DateTime.Today.AddDays(3) },
            new Cita { Matricula = "2020BDB", Marca = "Tesla", Modelo = "Model 3", Cilindrada = 1, Motor = Motor.Electrico, DniDueño = "50413269Y", FechaMatriculacion = new DateTime(2023, 6, 6), FechaInspeccion = DateTime.Today.AddDays(4) },

            new Cita { Matricula = "3030BDC", Marca = "Cupra", Modelo = "Formentor", Cilindrada = 1500, Motor = Motor.Gasolina, DniDueño = "19384756B", FechaMatriculacion = new DateTime(2022, 11, 13), FechaInspeccion = DateTime.Today.AddDays(5) },
            new Cita { Matricula = "4040BDF", Marca = "Volvo", Modelo = "XC40", Cilindrada = 2000, Motor = Motor.Hibrido, DniDueño = "28475639Y", FechaMatriculacion = new DateTime(2021, 3, 19), FechaInspeccion = DateTime.Today.AddDays(6) },
            new Cita { Matricula = "5050BDG", Marca = "Lexus", Modelo = "UX", Cilindrada = 2000, Motor = Motor.Hibrido, DniDueño = "37564928V", FechaMatriculacion = new DateTime(2020, 7, 27), FechaInspeccion = DateTime.Today.AddDays(7) },
            new Cita { Matricula = "6060BDH", Marca = "Mini", Modelo = "Cooper", Cilindrada = 1500, Motor = Motor.Gasolina, DniDueño = "46652819X", FechaMatriculacion = new DateTime(2018, 8, 2), FechaInspeccion = DateTime.Today.AddDays(8) },
            new Cita { Matricula = "7070BDJ", Marca = "Suzuki", Modelo = "Vitara", Cilindrada = 1400, Motor = Motor.Hibrido, DniDueño = "55741938N", FechaMatriculacion = new DateTime(2019, 9, 16), FechaInspeccion = DateTime.Today.AddDays(9) },
            new Cita { Matricula = "8080BDK", Marca = "Mitsubishi", Modelo = "ASX", Cilindrada = 1600, Motor = Motor.Gasolina, DniDueño = "64839271V", FechaMatriculacion = new DateTime(2017, 10, 7), FechaInspeccion = DateTime.Today.AddDays(10) },
            new Cita { Matricula = "9090BDL", Marca = "Alfa Romeo", Modelo = "Giulietta", Cilindrada = 1400, Motor = Motor.Gasolina, DniDueño = "73928164T", FechaMatriculacion = new DateTime(2016, 2, 24), FechaInspeccion = DateTime.Today.AddDays(11) },
            new Cita { Matricula = "1111BDM", Marca = "Jeep", Modelo = "Renegade", Cilindrada = 1300, Motor = Motor.Hibrido, DniDueño = "82017453M", FechaMatriculacion = new DateTime(2020, 5, 5), FechaInspeccion = DateTime.Today.AddDays(12) },
            new Cita { Matricula = "2222BDN", Marca = "Land Rover", Modelo = "Evoque", Cilindrada = 2000, Motor = Motor.Diesel, DniDueño = "91106342F", FechaMatriculacion = new DateTime(2019, 11, 11), FechaInspeccion = DateTime.Today.AddDays(13) },
            new Cita { Matricula = "3333BDP", Marca = "Jaguar", Modelo = "XE", Cilindrada = 2000, Motor = Motor.Diesel, DniDueño = "13579246T", FechaMatriculacion = new DateTime(2018, 12, 12), FechaInspeccion = DateTime.Today.AddDays(14) },

            new Cita { Matricula = "4444BDR", Marca = "Porsche", Modelo = "Macan", Cilindrada = 2000, Motor = Motor.Gasolina, DniDueño = "24681357B", FechaMatriculacion = new DateTime(2021, 10, 10), FechaInspeccion = DateTime.Today.AddDays(15) },
            new Cita { Matricula = "5555BDS", Marca = "Subaru", Modelo = "Impreza", Cilindrada = 1600, Motor = Motor.Gasolina, DniDueño = "35792468Y", FechaMatriculacion = new DateTime(2015, 1, 9), FechaInspeccion = DateTime.Today.AddDays(16) },
            new Cita { Matricula = "6666BDT", Marca = "Smart", Modelo = "Forfour", Cilindrada = 1, Motor = Motor.Electrico, DniDueño = "46813579T", FechaMatriculacion = new DateTime(2022, 2, 18), FechaInspeccion = DateTime.Today.AddDays(17) },
            new Cita { Matricula = "7777BDV", Marca = "MG", Modelo = "ZS", Cilindrada = 1, Motor = Motor.Electrico, DniDueño = "57924680P", FechaMatriculacion = new DateTime(2023, 3, 23), FechaInspeccion = DateTime.Today.AddDays(18) },
            new Cita { Matricula = "8888BDW", Marca = "BYD", Modelo = "Atto 3", Cilindrada = 1, Motor = Motor.Electrico, DniDueño = "68035791C", FechaMatriculacion = new DateTime(2024, 1, 12), FechaInspeccion = DateTime.Today.AddDays(19) },
            new Cita { Matricula = "9999BDX", Marca = "SsangYong", Modelo = "Korando", Cilindrada = 1600, Motor = Motor.Diesel, DniDueño = "79146802F", FechaMatriculacion = new DateTime(2019, 6, 15), FechaInspeccion = DateTime.Today.AddDays(20) },
            new Cita { Matricula = "1212BDY", Marca = "DS", Modelo = "DS 4", Cilindrada = 1600, Motor = Motor.Hibrido, DniDueño = "80257913B", FechaMatriculacion = new DateTime(2022, 5, 26), FechaInspeccion = DateTime.Today.AddDays(21) },
            new Cita { Matricula = "2323BDZ", Marca = "Abarth", Modelo = "595", Cilindrada = 1400, Motor = Motor.Gasolina, DniDueño = "91368024H", FechaMatriculacion = new DateTime(2017, 7, 31), FechaInspeccion = DateTime.Today.AddDays(22) },
            new Cita { Matricula = "3434BFB", Marca = "Infiniti", Modelo = "Q30", Cilindrada = 1500, Motor = Motor.Diesel, DniDueño = "12479135W", FechaMatriculacion = new DateTime(2016, 8, 28), FechaInspeccion = DateTime.Today.AddDays(23) },
            new Cita { Matricula = "4545BFC", Marca = "Saab", Modelo = "9-3", Cilindrada = 1900, Motor = Motor.Diesel, DniDueño = "23580246W", FechaMatriculacion = new DateTime(2011, 3, 3), FechaInspeccion = DateTime.Today.AddDays(24) },

            new Cita { Matricula = "5656BFD", Marca = "Chevrolet", Modelo = "Cruze", Cilindrada = 1800, Motor = Motor.Gasolina, DniDueño = "34691357C", FechaMatriculacion = new DateTime(2012, 4, 4), FechaInspeccion = DateTime.Today.AddDays(25) },
            new Cita { Matricula = "6767BFG", Marca = "Lancia", Modelo = "Delta", Cilindrada = 1600, Motor = Motor.Diesel, DniDueño = "45702468L", FechaMatriculacion = new DateTime(2010, 5, 5), FechaInspeccion = DateTime.Today.AddDays(26) },
            new Cita { Matricula = "7878BFH", Marca = "Chrysler", Modelo = "Voyager", Cilindrada = 2800, Motor = Motor.Diesel, DniDueño = "56813579Z", FechaMatriculacion = new DateTime(2009, 6, 6), FechaInspeccion = DateTime.Today.AddDays(27) },
            new Cita { Matricula = "8989BFJ", Marca = "Dodge", Modelo = "Caliber", Cilindrada = 2000, Motor = Motor.Gasolina, DniDueño = "67924680E", FechaMatriculacion = new DateTime(2008, 7, 7), FechaInspeccion = DateTime.Today.AddDays(28) },
            new Cita { Matricula = "9091BFK", Marca = "Isuzu", Modelo = "D-Max", Cilindrada = 2500, Motor = Motor.Diesel, DniDueño = "78035791B", FechaMatriculacion = new DateTime(2018, 10, 8), FechaInspeccion = DateTime.Today.AddDays(29) },
            new Cita { Matricula = "8181BFL", Marca = "Iveco", Modelo = "Daily", Cilindrada = 3000, Motor = Motor.Diesel, DniDueño = "89146802K", FechaMatriculacion = new DateTime(2016, 9, 9), FechaInspeccion = DateTime.Today.AddDays(3) },
            new Cita { Matricula = "7272BFM", Marca = "Maserati", Modelo = "Ghibli", Cilindrada = 3000, Motor = Motor.Gasolina, DniDueño = "90257913W", FechaMatriculacion = new DateTime(2020, 10, 20), FechaInspeccion = DateTime.Today.AddDays(6) },
            new Cita { Matricula = "6363BFN", Marca = "Ferrari", Modelo = "Roma", Cilindrada = 3900, Motor = Motor.Gasolina, DniDueño = "11368024K", FechaMatriculacion = new DateTime(2022, 12, 22), FechaInspeccion = DateTime.Today.AddDays(9) },
            new Cita { Matricula = "5454BFP", Marca = "Lamborghini", Modelo = "Urus", Cilindrada = 4000, Motor = Motor.Gasolina, DniDueño = "22479135Q", FechaMatriculacion = new DateTime(2021, 11, 25), FechaInspeccion = DateTime.Today.AddDays(12) },
            new Cita { Matricula = "4546BFR", Marca = "Polestar", Modelo = "2", Cilindrada = 1, Motor = Motor.Electrico, DniDueño = "33580246Q", FechaMatriculacion = new DateTime(2023, 9, 14), FechaInspeccion = DateTime.Today.AddDays(15) }
        };
    }
}