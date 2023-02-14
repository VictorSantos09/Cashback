﻿using Cashback.Application.Dto;
using Cashback.Application.Extensions;
using Cashback.Domain.Entities;
using Cashback.Domain.Interfaces;
using Cashback.Repository.Interfaces;

namespace Cashback.Application.Services
{
    public class ProcedimentService
    {
        private readonly IBaseRepository<UserEntity> _userRepository;

        public ProcedimentService(IBaseRepository<UserEntity> userRepository)
        {
            _userRepository = userRepository;
        }

        public BaseDto Register(IProcediment procediment, Guid id)
        {
            if (string.IsNullOrEmpty(procediment.CPFClient) || procediment.CPFClient.Length > 11)
                return BaseDtoExtension.InvalidValue();

            if (string.IsNullOrEmpty(procediment.Value.ToString()))
                return BaseDtoExtension.InvalidValue();

            if (string.IsNullOrEmpty(procediment.Name))
                return BaseDtoExtension.Error(406, "Procedimento inválido");

            var procedimentEntity = new ProcedimentEntity(procediment.Value, procediment.Name,
                procediment.CPFClient, procediment.NamePacient);

            var user = _userRepository.GetById(id);

            if (user == null)
                return BaseDtoExtension.NotFound();

            user.Procediments.Add(procedimentEntity);

            var cashAmount = CalculateService.CalculateCashback(procediment.Value);

            user.Account.Balance += cashAmount;

            return BaseDtoExtension.Create(200, $"PROCEDIMENTO {procediment.Name} REGISTRADO PARA O PACIENTE " +
                $"{procediment.NamePacient} NO VALOR DE {procediment.Value}");
        }
    }
}