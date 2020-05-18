using System;

using NthDimension.Network;
using NthDimension.Procedural;
using NthDimension.Procedural.Text;
using NthDimension.Procedural.Text.WordRepos;

namespace NthStudio.Game
{
    public class AvatarFactory
    {
        private static WordGenerator generatorMaleNames = new WordGenerator(" ", Casing.PascalCase, new WordBank[]
                                                                                               {
                                                                                                    new WordBank(WordType.FirstName, "FirstNames",  new MaleFirstNameRepository()),
                                                                                                    new WordBank(WordType.LastName,  "LastNames",   new LastNamesRepository())
                                                                                               });

        private static WordGenerator generatorFemaleNames = new WordGenerator(" ", Casing.PascalCase, new WordBank[]
                                                                                               {
                                                                                                    new WordBank(WordType.FirstName, "FirstNames",  new FemaleFirstNameRepository()),
                                                                                                    new WordBank(WordType.LastName,  "LastNames",    new LastNamesRepository())
                                                                                               });
        private static Random        rnd = new Random(128812);
        public static AvatarInfoDesc GenerateRandom(enuAvatarSex gender = enuAvatarSex.Random)
        {
            AvatarInfoDesc      ret;
            
            Guid                guid        = Guid.NewGuid();
            string              faceType    = "generic";
            int                 sexRnd      = rnd.Next(0, 3);
            enuAvatarSex        sex         = (gender == enuAvatarSex.Random ? (((sexRnd == 0 || sexRnd == 2) ? 
                                                                                    enuAvatarSex.Male : 
                                                                                    enuAvatarSex.Female)) 
                                                                             : gender);
            string              sexToString = sex == enuAvatarSex.Male ? "male" : "female";

            string[]            bodyTypes   = new string[] { "normal", "fit", "fat" };
            int                 bodyRnd     = rnd.Next(0, 2);
            string              bodyType    = bodyTypes[bodyRnd];

            string              name        = sex == enuAvatarSex.Male ? generatorMaleNames.Generate() : generatorFemaleNames.Generate();

            string[]            hairTypes   = sex == enuAvatarSex.Male ? new string[] { "short", "medium", "tail" } 
                                                                       : new string[] { "short", "medium", "long", "tail" };
            int                 hairRnd     = sex == enuAvatarSex.Male ? rnd.Next(0, 2) : rnd.Next(0, 3);
            string              hairType    = sex == enuAvatarSex.Male ? hairTypes[hairRnd] : hairTypes[hairRnd];


            string[]            hairColors  = sex == enuAvatarSex.Male ? new string[] { "black", "blonde", "brown", "black" }
                                                                       : new string[] { "black", "blonde", "brown", "red" };
            string              hairColor   = hairColors[rnd.Next(0, 3)];


            string[]            shirtColors = sex == enuAvatarSex.Male ? new string[] { "blue", "brown", "red", "multicolor", "red" }
                                                                       : new string[] { "black", "blue", "green", "red", "white" };
            string              shirtColor  = sex == enuAvatarSex.Male ? shirtColors[rnd.Next(0, 4)]
                                                                       : shirtColors[rnd.Next(0, 4)];

            string[]            pantsColors = sex == enuAvatarSex.Male ? new string[] { "black", "blue", "brown", "green", "red" }
                                                                       : new string[] { "black", "blue", "green", "red", "white" };
            string              pantsColor  = pantsColors[rnd.Next(0,4)];

            string[]            shoesColors = sex == enuAvatarSex.Male ? new string[] { "black", "blue", "brown", "green" }
                                                                       : new string[] { "black", "blue", "green", "red", "white" };
            string              shoesColor = sex == enuAvatarSex.Male ? shoesColors[rnd.Next(0, 3)] : shoesColors[rnd.Next(0, 4)];

            string              spawnPos    = string.Format(@"{0}|{1}|{2}", rnd.Next(-60, 60), "0", rnd.Next(0, 50));

            //var r = AvatarPresets.GetAvatar((sex == enuAvatarSex.Male ? "male" : "female"), bodyType, faceType, hairType, hairColor, shirtColor, pantsColor, shoesColor);
            //r.AvatarName = name;

            //return r;

            

            ret = new AvatarInfoDesc(guid.ToString(),
                                     sex,
                                     bodyType,
                                     faceType,
                                     name,
                                     string.Format(@"characters\{0}\body_{1}_generic.xmd", sexToString, bodyType),
                                     string.Format(@"characters\{0}\fit_generic.xmf", sexToString),
                                     string.Format("{0}|{0}|{0}", AvatarPresets.avatarSize.ToString("###.##")),
                                     @"-4.371139E-08|0|-1|0|0|1|0|0|1|0|-4.371139E-08|0|0|0|0|1",
                                     spawnPos,
                                     new System.Collections.Generic.List<AttachmentInfoDesc>()
                                     {
                                         new AttachmentInfoDesc(AvatarPresets.GetFaceAttachment("local", sexToString, bodyType, faceType))          { Orientation = "1|0|0|0|0|1|0|0|0|0|1|0|0|0|0|1" },
                                         new AttachmentInfoDesc(AvatarPresets.GetHairAttachment(sexToString, hairType, hairColor))                  { Orientation = "1|0|0|0|0|1|0|0|0|0|1|0|0|0|0|1" },
                                         new AttachmentInfoDesc(AvatarPresets.GetShirtAttachment(sexToString, bodyType, faceType, shirtColor))      { Orientation = "1|0|0|0|0|1|0|0|0|0|1|0|0|0|0|1" },
                                         new AttachmentInfoDesc(AvatarPresets.GetLeggingsAttachment(sexToString, bodyType, faceType, pantsColor))   { Orientation = "1|0|0|0|0|1|0|0|0|0|1|0|0|0|0|1" },
                                         new AttachmentInfoDesc(AvatarPresets.GetShoesAttachment(sexToString, bodyType, faceType, shoesColor))      { Orientation = "1|0|0|0|0|1|0|0|0|0|1|0|0|0|0|1" },

                                     });

            ret.Attachments[0].Material = @"characters\defaultGenericFace.xmf"; // Face Material

            return ret;
        }
    }
}
