using CSharpFunctionalExtensions;
using Itv.Cache;
using Itv.Config;
using Itv.Enums;
using Itv.Errors;
using Itv.Errors.Common;
using Itv.Models;
using Itv.Repository.Common;
using Itv.Validator.Common;

namespace Itv.Services.Citas;

public class CitaService(
        IValidator<Cita> validador,
        ICitaRepository repository,
        ICache<int,Cita> cache
    ) : ICitaService {
    
    public IEnumerable<Cita> GetAll(int pagina = 1, int tamPagina = 5, bool isDeleteInclude = true, string campoBusqueda = "") {
        return repository.GetAll(pagina, tamPagina, isDeleteInclude, campoBusqueda);
    }

    public Result<Cita, DomainError> GetById(int id) {
        if (cache.Get(id) is { } cached) {
            return Result.Success<Cita, DomainError>(cached);
        }

        var res = repository.GetById(id);
        if (res.IsFailure) {
            return Result.Failure<Cita, DomainError>(CitaErrors.NotFoundCitasError());
        }

        cache.Add(GetById(id).Value.Id, GetById(id).Value);
        return Result.Success<Cita, DomainError>(GetById(id).Value);
    }

    public Result<IEnumerable<Cita>, DomainError> GetByDateInspeccion(DateTime inicio, DateTime? fin, bool isDeleteInclude = true) {
        return repository.GetByDateInspeccion(inicio, fin, isDeleteInclude);
    }

    public Result<IEnumerable<Cita>, DomainError> GetByTipoMotor(Motor motor, bool isDeleteInclude = true) {
        return repository.GetByTipoMotor(motor, isDeleteInclude);
    }

    public Result<Cita, DomainError> Create(Cita cita) {
        return validador.Validate(cita)
            .Bind(c => repository.Create(cita))
            .Tap(creada => cache.Add(creada.Id, creada));
    }

    public Result<Cita, DomainError> Update(int id, Cita cita) {
        return ComprobarExistencia(id)
            .Bind(c => repository.Update(id, cita))
            .Tap(c => cache.Remove(id))
            .Tap(c => cache.Add(id, cita));
    }

    public Result<Cita, DomainError> Delete(int id) {
        if (!Configuracion.UseLogicalDelete) {
            return ComprobarExistencia(id)
                .Bind(c => repository.DeleteHard(id))
                .Tap(c => cache.Remove(id));
        }

        return ComprobarExistencia(id)
            .Bind(c => repository.Delete(id))
            .Tap(c => cache.Remove(id));
    }

    public bool DeleteAll() {
        return repository.DeleteAll();
    }

    private Result<Cita, DomainError> ComprobarExistencia(int id) {
        var res = repository.GetById(id);
        return res.IsSuccess
            ? Result.Success<Cita, DomainError>(res.Value)
            : Result.Failure<Cita, DomainError>(CitaErrors.NotFoundCitasError());
    }
}