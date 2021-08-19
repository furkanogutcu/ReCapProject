﻿using System.Collections.Generic;
using System.Linq;
using Business.Abstract;
using Business.Constants;
using Business.ValidationRules.FluentValidation;
using Core.Aspects.Autofac.Validation;
using Core.Utilities.Business;
using Core.Utilities.Results;
using DataAccess.Abstract;
using Entities.Concrete;

namespace Business.Concrete
{
    public class ColorManager : IColorService
    {
        private readonly IColorDal _colorDal;

        public ColorManager(IColorDal colorDal)
        {
            _colorDal = colorDal;
        }

        public IDataResult<Color> GetColorById(int id)
        {
            return new SuccessDataResult<Color>(_colorDal.Get(c => c.Id == id), Messages.ColorListed);
        }

        public IDataResult<List<Color>> GetAll()
        {
            return new SuccessDataResult<List<Color>>(_colorDal.GetAll(), Messages.ColorsListed);
        }

        [ValidationAspect(typeof(ColorValidator))]
        public IResult Add(Color color)
        {
            var rulesResult = BusinessRules.Run(CheckIfColorNameExist(color.Name));
            if (rulesResult != null)
            {
                return rulesResult;
            }

            _colorDal.Add(color);
            return new SuccessResult(Messages.ColorAdded);
        }

        [ValidationAspect(typeof(ColorValidator))]
        public IResult Update(Color color)
        {
            var rulesResult = BusinessRules.Run(CheckIfColorIdExist(color.Id),
                CheckIfColorNameExist(color.Name));
            if (rulesResult != null)
            {
                return rulesResult;
            }

            _colorDal.Update(color);
            return new SuccessResult(Messages.ColorUpdated);
        }


        public IResult Delete(int colorId)
        {
            var rulesResult = BusinessRules.Run(CheckIfColorIdExist(colorId));
            if (rulesResult != null)
            {
                return rulesResult;
            }

            var deletedColor = _colorDal.Get(c => c.Id == colorId);
            _colorDal.Delete(deletedColor);
            return new SuccessResult(Messages.ColorDeleted);
        }

        //Business Rules

        private IResult CheckIfColorIdExist(int colorId)
        {
            var result = _colorDal.GetAll(c => c.Id == colorId).Any();
            if (!result)
            {
                return new ErrorResult(Messages.ColorNotExist);
            }
            return new SuccessResult();
        }

        private IResult CheckIfColorNameExist(string colorName)
        {
            var result = _colorDal.GetAll(c => c.Name == colorName).Any();
            if (result)
            {
                return new ErrorResult(Messages.ColorNameExist);
            }
            return new SuccessResult();
        }
    }
}
