﻿using System.Collections.Generic;
using System.Linq;
using Business.Abstract;
using Business.BusinessAspects.Autofac;
using Business.Constants;
using Business.ValidationRules.FluentValidation;
using Core.Aspects.Autofac.Caching;
using Core.Aspects.Autofac.Validation;
using Core.Utilities.Business;
using Core.Utilities.Results;
using DataAccess.Abstract;
using Entities.Concrete;
using Entities.DTOs;

namespace Business.Concrete
{
    public class CarManager : ICarService
    {
        private readonly ICarDal _carDal;

        public CarManager(ICarDal carDal)
        {
            _carDal = carDal;
        }

        [SecuredOperation("admin,car.all,car.list")]
        [CacheAspect(10)]
        public IDataResult<Car> GetById(int id)
        {
            return new SuccessDataResult<Car>(_carDal.Get(c => c.Id == id), Messages.CarListed);
        }

        [SecuredOperation("admin,car.all,car.list")]
        [CacheAspect(10)]
        public IDataResult<List<Car>> GetAll()
        {
            return new SuccessDataResult<List<Car>>(_carDal.GetAll(), Messages.CarsListed);
        }

        [SecuredOperation("admin,car.all,car.add")]
        [ValidationAspect(typeof(CarValidator))]
        [CacheRemoveAspect("ICarService.Get")]
        public IResult Add(Car car)
        {
            _carDal.Add(car);
            return new SuccessResult(Messages.CarAdded);
        }

        [SecuredOperation("admin,car.all,car.update")]
        [ValidationAspect(typeof(CarValidator))]
        [CacheRemoveAspect("ICarService.Get")]
        public IResult Update(Car car)
        {
            var rulesResult = BusinessRules.Run(CheckIfCarIdExist(car.Id));
            if (rulesResult != null)
            {
                return rulesResult;
            }
            _carDal.Update(car);
            return new SuccessResult(Messages.CarUpdated);
        }

        [SecuredOperation("admin,car.all,car.delete")]
        [CacheRemoveAspect("ICarService.Get")]
        public IResult Delete(int carId)
        {
            var rulesResult = BusinessRules.Run(CheckIfCarIdExist(carId));
            if (rulesResult != null)
            {
                return rulesResult;
            }

            var deletedCar = _carDal.Get(c => c.Id == carId);
            _carDal.Delete(deletedCar);
            return new SuccessResult(Messages.CarDeleted);
        }

        [SecuredOperation("admin,car.all,car.list")]
        [CacheAspect(10)]
        public IDataResult<List<Car>> GetCarsByBrandId(int id)
        {
            return new SuccessDataResult<List<Car>>(_carDal.GetAll(c => c.BrandId == id), Messages.CarsListed);
        }

        [SecuredOperation("admin,car.all,car.list")]
        [CacheAspect(10)]
        public IDataResult<List<Car>> GetCarsByColorId(int id)
        {
            return new SuccessDataResult<List<Car>>(_carDal.GetAll(c => c.ColorId == id), Messages.CarsListed);
        }

        [SecuredOperation("admin,car.all,car.list")]
        [CacheAspect(10)]
        public IDataResult<List<CarDetailDto>> GetCarsWithDetails()
        {
            return new SuccessDataResult<List<CarDetailDto>>(_carDal.GetCarDetails(), Messages.CarsListed);
        }

        [SecuredOperation("admin,car.all,car.list")]
        [CacheAspect(10)]
        public IDataResult<CarDetailDto> GetCarDetails(int carId)
        {
            return new SuccessDataResult<CarDetailDto>(_carDal.GetCarDetails(c => c.Id == carId).SingleOrDefault(), Messages.CarListed);
        }

        [SecuredOperation("admin,car.all,car.list")]
        [CacheAspect(10)]
        public IDataResult<List<CarDetailDto>> GetCarsByBrandIdWithDetails(int brandId)
        {
            return new SuccessDataResult<List<CarDetailDto>>(_carDal.GetCarDetails(c=>c.BrandId==brandId), Messages.CarsListed);
        }

        [SecuredOperation("admin,car.all,car.list")]
        [CacheAspect(10)]
        public IDataResult<List<CarDetailDto>> GetCarsByColorIdWithDetails(int colorId)
        {
            return new SuccessDataResult<List<CarDetailDto>>(_carDal.GetCarDetails(c => c.ColorId == colorId), Messages.CarsListed);
        }

        [SecuredOperation("admin,car.all,car.list")]
        [CacheAspect(10)]
        public IDataResult<List<CarDetailDto>> GetCarsByFilterWithDetails(int brandId, int colorId)
        {
            return new SuccessDataResult<List<CarDetailDto>>(_carDal.GetCarDetails(c => c.BrandId == brandId && c.ColorId == colorId), Messages.CarsListed);
        }

        //Business Rules

        private IResult CheckIfCarIdExist(int carId)
        {
            var result = _carDal.GetAll(c => c.Id == carId).Any();
            if (!result)
            {
                return new ErrorResult(Messages.CarNotExist);
            }
            return new SuccessResult();
        }
    }
}
