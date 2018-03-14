﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bytes2you.Validation;
using SquidsMovieApp.Data.Contracts;

namespace SquidsMovieApp.Data.Models.Abstract
{
    public abstract class Person : IPerson
    {
        private string firstName;
        private string lastName;
        private int age;


        protected Person(string firstName, string lastName, int age)
        {
            this.FirstName = firstName;
            this.LastName = lastName;
            this.Age = age;
        }

        public string Role { get; set; }

        public string FirstName
        {
            get
            {
                return this.firstName;
            }
            set
            {
                Guard.WhenArgument(value, "Actor First Name")
                    .IsNullOrEmpty()
                    .Throw();
                this.firstName = value;
            }
        }

        public string LastName
        {
            get
            {
                return this.lastName;
            }
            set
            {
                Guard.WhenArgument(value, "Actor First Name")
                    .IsNullOrEmpty()
                    .Throw();
                this.lastName = value;
            }
        }

        public int Age
        {
            get
            {
                return this.age;
            }
            set
            {
                Guard.WhenArgument(value, "Age")
                    .IsLessThan(3)
                    .IsGreaterThan(120)
                    .Throw();
                this.age = value;
            }
        }
    }
}
