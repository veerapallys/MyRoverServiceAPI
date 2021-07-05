using MyRoverServiceAPI;
using System;
using System.Collections.Generic;

namespace MyRoversServiceAPI.UnitTests.Services
{
    public static class ServiceTestData
    {
        public static MarsRoverPhotosManifest GetManifest()
        {
            return new MarsRoverPhotosManifest
            {
                PhotosManifest = new RoverPhotosManifest
                {
                    LandingDate = DateTime.Parse("2017-06-30"),
                    MaxDate = DateTime.Parse("2021-07-03"),
                    Status = "Active",
                    TotalPhotos = 45,
                    Photos = new List<Photo>()
                    {
                        new Photo
                        {
                            EarthDate = DateTime.Parse("2021-06-30"),
                            TotalPhotos = 30
                        },
                        new Photo
                        {
                            EarthDate = DateTime.Parse("2018-05-10"),
                            TotalPhotos = 5
                        },
                        new Photo
                        {
                            EarthDate = DateTime.Parse("2019-07-15"),
                            TotalPhotos = 5
                        },
                        new Photo
                        {
                            EarthDate = DateTime.Parse("2017-04-20"),
                            TotalPhotos = 5
                        }
                    }
                }
            };
        }

        public static MarsRoverPhotos GetMarsRoverPhotosPageOne()
        {
            return new MarsRoverPhotos
            {
                Photos = new RoverPhotos[]
                {
                    new RoverPhotos
                    {
                        EarthDate=DateTime.Parse("2021-06-30"),
                        Id=1,
                        ImageSourceUrl="http://mynasa.gov/1.jpg"
                    },
                    new RoverPhotos
                    {
                        EarthDate=DateTime.Parse("2021-06-30"),
                        Id=2,
                        ImageSourceUrl="http://mynasa.gov/2.jpg"
                    },
                    new RoverPhotos
                    {
                        EarthDate=DateTime.Parse("2021-06-30"),
                        Id=3,
                        ImageSourceUrl="http://mynasa.gov/3.jpg"
                    }
                }
            };
        }

        public static MarsRoverPhotos GetMarsRoverPhotosPageTwo()
        {
            return new MarsRoverPhotos
            {
                Photos = new RoverPhotos[]
                {
                    new RoverPhotos
                    {
                        EarthDate=DateTime.Parse("2021-06-30"),
                        Id=4,
                        ImageSourceUrl="http://mynasa.gov/4.jpg"
                    },
                    new RoverPhotos
                    {
                        EarthDate=DateTime.Parse("2021-06-30"),
                        Id=5,
                        ImageSourceUrl="http://mynasa.gov/5.jpg"
                    },
                    new RoverPhotos
                    {
                        EarthDate=DateTime.Parse("2021-06-30"),
                        Id=6,
                        ImageSourceUrl="http://mynasa.gov/6.jpg"
                    }
                }
            };
        }

    }
}
