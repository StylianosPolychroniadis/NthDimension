using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NthDimension.Network
{
    public static class AvatarPresets
    {
        public const float avatarSize = .02f; 

        public static AvatarInfoDesc MaleFit_Generic_0()
        {
            #region MaleFit Generic (master)
            string m0gender = "male";
            string m0body = "normal";
            string m0face = "generic";
            string m0hairtype = "short"; //"short"
            string m0haircolor = "brown";
            string m0shirtcolor = "blue";
            string m0leggingscolor = "red";
            string m0shoescolor = "red";

            #region Face Attachment
            AttachmentInfoDesc MaleFit_Generic_0_attFace = new AttachmentInfoDesc();
            MaleFit_Generic_0_attFace = GetFaceAttachment("local", m0gender, m0body, m0face);
            #endregion

            #region Hair Attachment
            AttachmentInfoDesc MaleFit_Generic_0_attHair = new AttachmentInfoDesc();
            MaleFit_Generic_0_attHair = GetHairAttachment(m0gender, m0hairtype, m0haircolor);
            #endregion

            #region Shirt Attachment
            AttachmentInfoDesc MaleFit_Generic_0_attShirt = new AttachmentInfoDesc();
            MaleFit_Generic_0_attShirt = GetShirtAttachment(m0gender, m0body, m0face, m0shirtcolor);
            #endregion

            #region Pants Attachment
            AttachmentInfoDesc MaleFit_Generic_0_attPants = new AttachmentInfoDesc();
            MaleFit_Generic_0_attPants = GetLeggingsAttachment(m0gender, m0body, m0face, m0leggingscolor);
            #endregion

            #region Shoes Attachment
            AttachmentInfoDesc MaleFit_Generic_0_attShoes = new AttachmentInfoDesc();
            MaleFit_Generic_0_attShoes = GetShoesAttachment(m0gender, m0body, m0face, m0shoescolor);
            #endregion

            AvatarInfoDesc MaleFit_Generic_0 = new AvatarInfoDesc(new Guid().ToString(),
                                                enuAvatarSex.Male,
                                                string.Format(@"{0}", m0body),
                                                @"generic",
                                                @"Avatar",
                                                string.Format(@"characters\male\body_{0}_generic.xmd", m0body),                                   // Body is ALWAYS .xmd
                                                string.Format(@"characters\male\{0}_generic.xmf", m0body),
                                                string.Format("{0}|{0}|{0}", avatarSize.ToString("###.##")),
                                                @"-4.371139E-08|0|-1|0|0|1|0|0|1|0|-4.371139E-08|0|0|0|0|1",
                                                @"140|0|50",
                                                new List<AttachmentInfoDesc>()
                                                    {
                                                        MaleFit_Generic_0_attFace,           // 0 is ALWAYS Face
                                                        MaleFit_Generic_0_attHair,           // 1 is ALWAYS Hair
                                                        MaleFit_Generic_0_attShirt,          // 2 is ALWAYS Shirt
                                                        MaleFit_Generic_0_attPants,          // 3 is ALWAYS Pants
                                                        MaleFit_Generic_0_attShoes           // 4 is ALWAYS Shoes     
                                                    });
            #endregion MaleFit Generic (master)
            return MaleFit_Generic_0;
        }
        public static AvatarInfoDesc FemaleFit_Generic_0()
        {
            #region FemaleFit Generic Ebony (master)
            string f0gender = "female";
            string f0body = "fit";
            string f0face = "generic";
            string f0hairtype = "tail"; //"short"
            string f0haircolor = "blonde";
            string f0shirtcolor = "white";
            string f0leggingscolor = "white";
            string f0shoescolor = "red";




            #region Face Attachment
            AttachmentInfoDesc FemaleFit_Generic_attFace = new AttachmentInfoDesc();
            FemaleFit_Generic_attFace = GetFaceAttachment("local", f0gender, f0body, f0face);
            #endregion

            #region Hair Attachment
            AttachmentInfoDesc FemaleFit_Generic_attHair = new AttachmentInfoDesc();
            FemaleFit_Generic_attHair = GetHairAttachment(f0gender, f0hairtype, f0haircolor);
            #endregion

            #region Shirt Attachment
            AttachmentInfoDesc FemaleFit_Generic_attShirt = new AttachmentInfoDesc();
            FemaleFit_Generic_attShirt = GetShirtAttachment(f0gender, f0body, f0face, f0shirtcolor);
            #endregion

            #region Pants Attachment
            AttachmentInfoDesc FemaleFit_Generic_attPants = new AttachmentInfoDesc();
            FemaleFit_Generic_attPants = GetLeggingsAttachment(f0gender, f0body, f0face, f0leggingscolor);
            #endregion

            #region Shoes Attachment
            AttachmentInfoDesc FemaleFit_Generic_attShoes = new AttachmentInfoDesc();
            FemaleFit_Generic_attShoes = GetShoesAttachment(f0gender, f0body, f0face, f0shoescolor);
            #endregion

           
            AvatarInfoDesc FemaleFit = new AvatarInfoDesc(new Guid().ToString(),
                                                enuAvatarSex.Female,
                                                string.Format(@"{0}", f0body),
                                                @"generic",
                                                @"Avatar",
                                                string.Format(@"characters\female\body_{0}_generic.xmd", f0body),                                   // Body is ALWAYS .xmd
                                                string.Format(@"characters\female\{0}_generic.xmf", f0body),
                                                string.Format("{0}|{0}|{0}", avatarSize.ToString("###.##")),
                                                @"-4.371139E-08|0|-1|0|0|1|0|0|1|0|-4.371139E-08|0|0|0|0|1",
                                                @"140|0|50",
                                                new List<AttachmentInfoDesc>()
                                                    {
                                                        FemaleFit_Generic_attFace,           // 0 is ALWAYS Face
                                                        FemaleFit_Generic_attHair,           // 1 is ALWAYS Hair
                                                        FemaleFit_Generic_attShirt,          // 2 is ALWAYS Shirt
                                                        FemaleFit_Generic_attPants,          // 3 is ALWAYS Pants
                                                        FemaleFit_Generic_attShoes           // 4 is ALWAYS Shoes     
                                                    });





            #endregion
            return FemaleFit;
        }
        public static AvatarInfoDesc FemaleFit_Generic_Ebony_00()
        {
            #region FemaleFit Generic Ebony (master)
            string f0gender = "female";
            string f0body = "fit";
            string f0face = "generic";
            string f0hairtype = "tail"; //"short"
            string f0haircolor = "blonde";
            string f0shirtcolor = "white";
            string f0leggingscolor = "white";
            string f0shoescolor = "red";




            #region Face Attachment
            AttachmentInfoDesc FemaleFit_Generic_Ebony_attFace = new AttachmentInfoDesc();
            FemaleFit_Generic_Ebony_attFace = GetFaceAttachment("local", f0gender, f0body, f0face);
            #endregion

            #region Hair Attachment
            AttachmentInfoDesc FemaleFit_Generic_Ebony_attHair = new AttachmentInfoDesc();
            FemaleFit_Generic_Ebony_attHair = GetHairAttachment(f0gender, f0hairtype, f0haircolor);
            #endregion

            #region Shirt Attachment
            AttachmentInfoDesc FemaleFit_Generic_Ebony_attShirt = new AttachmentInfoDesc();
            FemaleFit_Generic_Ebony_attShirt = GetShirtAttachment(f0gender, f0body, f0face, f0shirtcolor);
            #endregion

            #region Pants Attachment
            AttachmentInfoDesc FemaleFit_Generic_Ebony_attPants = new AttachmentInfoDesc();
            FemaleFit_Generic_Ebony_attPants = GetLeggingsAttachment(f0gender, f0body, f0face, f0leggingscolor);
            #endregion

            #region Shoes Attachment
            AttachmentInfoDesc FemaleFit_Generic_Ebony_attShoes = new AttachmentInfoDesc();
            FemaleFit_Generic_Ebony_attShoes = GetShoesAttachment(f0gender, f0body, f0face, f0shoescolor);
            #endregion

            /*
             string.Format(@"{0}", m0body),
                                                @"generic",
                                                @"Avatar",
                                                string.Format(@"characters\male\body_{0}_generic.xmd", m0body),                                   // Body is ALWAYS .xmd
                                                string.Format(@"characters\male\{0}_generic.xmf", m0body),
            */
            AvatarInfoDesc FemaleFit_Generic_Ebony_00 = new AvatarInfoDesc(new Guid().ToString(),
                                                enuAvatarSex.Female,
                                                //@"fit",
                                                //@"generic",
                                                //@"Avatar",
                                                //@"characters\female\body_fit_generic.xmd",                                   // Body is ALWAYS .xmd
                                                //@"characters\female\fit_generic.xmf",
                                                string.Format(@"{0}", f0body),
                                                @"generic",
                                                @"Avatar",
                                                string.Format(@"characters\female\body_{0}_generic.xmd", f0body),                                   // Body is ALWAYS .xmd
                                                string.Format(@"characters\female\{0}_generic.xmf", f0body),
                                                string.Format("{0}|{0}|{0}", avatarSize.ToString("###.##")),
                                                @"-4.371139E-08|0|-1|0|0|1|0|0|1|0|-4.371139E-08|0|0|0|0|1",
                                                @"140|0|50",
                                                new List<AttachmentInfoDesc>()
                                                    {
                                                        FemaleFit_Generic_Ebony_attFace,           // 0 is ALWAYS Face
                                                        FemaleFit_Generic_Ebony_attHair,           // 1 is ALWAYS Hair
                                                        FemaleFit_Generic_Ebony_attShirt,          // 2 is ALWAYS Shirt
                                                        FemaleFit_Generic_Ebony_attPants,          // 3 is ALWAYS Pants
                                                        FemaleFit_Generic_Ebony_attShoes           // 4 is ALWAYS Shoes     
                                                    });



            #region older values
            //FemaleFit_Generic_Ebony_attFace.Name                          = "face";
            //FemaleFit_Generic_Ebony_attFace.Matrix                        = "5";
            //FemaleFit_Generic_Ebony_attFace.Material                      = @"characters\face_female_ebony_00.xmf";
            //FemaleFit_Generic_Ebony_attFace.Mesh                          = @"characters\female\face_fit_generic.obj";                                                         // Face is ALWAYS .OBJ
            //FemaleFit_Generic_Ebony_attFace.Size                          = "0.025|0.025|0.025";
            //FemaleFit_Generic_Ebony_attFace.Vertex                        = "head";
            //FemaleFit_Generic_Ebony_attFace.Orientation                   = "-4.371139E-08|0|-1|0|0|1|0|0|1|0|-4.371139E-08|0|0|0|0|1";
            ////FemaleFit_attFace.Offset                                     = "0|-13|8";// = "0|-15|10.5"; // "0|-0.18|0.11";
            //FemaleFit_Generic_Ebony_attFace.Offset                        = "0|-13.3|7.8";       // Female Generic Face

            //FemaleFit_Generic_Ebony_attHair.Name                          = "hair";
            //FemaleFit_Generic_Ebony_attHair.Matrix                        = "5";
            //FemaleFit_Generic_Ebony_attHair.Material                      = @"characters\female\hairstyle_red.xmf";
            //FemaleFit_Generic_Ebony_attHair.Mesh                          = @"characters\female\hairstyle_long.obj"; 
            //FemaleFit_Generic_Ebony_attHair.Size                          = "0.025|0.025|0.025";
            //FemaleFit_Generic_Ebony_attHair.Vertex                        = "hair";
            //FemaleFit_Generic_Ebony_attHair.Orientation                   = "-4.371139E-08|0|-1|0|0|1|0|0|1|0|-4.371139E-08|0|0|0|0|1";
            ////FemaleFit_Generic_Ebony_attHair.Offset                      = "0.35|-0.25|0.35";          // Female Short
            ////FemaleFit_Generic_Ebony_attHair.Offset                      = "0.5|-0.4|0";               // Female Medium
            ////FemaleFit_Generic_Ebony_attHair.Offset                      = "0|0|-1";                   // Female Long
            //FemaleFit_Generic_Ebony_attHair.Offset                        = "-2.5|0|-3.5";              // Female Tail

            //FemaleFit_Generic_Ebony_attShirt.Name                         = "tshirt0";
            //FemaleFit_Generic_Ebony_attShirt.Matrix                       = "-1";
            //FemaleFit_Generic_Ebony_attShirt.Material                     = @"characters\female\apparel\fit_tshirt_red.xmf";
            //FemaleFit_Generic_Ebony_attShirt.Mesh                         = @"characters\female\apparel\fit_generic_tshirt.xmd";
            //FemaleFit_Generic_Ebony_attShirt.Size                         = "0.025|0.025|0.025";
            //FemaleFit_Generic_Ebony_attShirt.Vertex                       = "spine2";
            //FemaleFit_Generic_Ebony_attShirt.Orientation                  = "-4.371139E-08|0|-1|0|0|1|0|0|1|0|-4.371139E-08|0|0|0|0|1";
            //FemaleFit_Generic_Ebony_attShirt.Offset                       = "0|0|0";
            //FemaleFit_Generic_Ebony_attShirt.Animation_Idle               = "female_tshirt_fit_idle";
            //FemaleFit_Generic_Ebony_attShirt.Animation_Walk               = "female_tshirt_fit_walk";
            //FemaleFit_Generic_Ebony_attShirt.Animation_Sit                = "female_tshirt_fit_sit";
            //FemaleFit_Generic_Ebony_attShirt.Animation_SitIdle            = "female_tshirt_fit_sidle";
            //FemaleFit_Generic_Ebony_attShirt.Animation_Stand              = "female_tshirt_fit_stand";
            //FemaleFit_Generic_Ebony_attShirt.Animation_Wave               = "female_tshirt_fit_wave";

            //FemaleFit_Generic_Ebony_attPants.Name                         = "leggings0";
            //FemaleFit_Generic_Ebony_attPants.Matrix                       = "-1";
            //FemaleFit_Generic_Ebony_attPants.Material                     = @"characters\female\apparel\fit_leggings_red.xmf";
            //FemaleFit_Generic_Ebony_attPants.Mesh                         = @"characters\female\apparel\fit_generic_leggings.xmd";
            //FemaleFit_Generic_Ebony_attPants.Size                         = "0.025|0.025|0.025";
            //FemaleFit_Generic_Ebony_attPants.Vertex                       = "spine2";
            //FemaleFit_Generic_Ebony_attPants.Orientation                  = "-4.371139E-08|0|-1|0|0|1|0|0|1|0|-4.371139E-08|0|0|0|0|1";
            //FemaleFit_Generic_Ebony_attPants.Offset                       = "0|0|0";
            //FemaleFit_Generic_Ebony_attPants.Animation_Idle               = "female_leggings_fit_idle";
            //FemaleFit_Generic_Ebony_attPants.Animation_Walk               = "female_leggings_fit_walk";
            //FemaleFit_Generic_Ebony_attPants.Animation_Sit                = "female_leggings_fit_sit";
            //FemaleFit_Generic_Ebony_attPants.Animation_SitIdle            = "female_leggings_fit_sidle";
            //FemaleFit_Generic_Ebony_attPants.Animation_Stand              = "female_leggings_fit_stand";
            //FemaleFit_Generic_Ebony_attPants.Animation_Wave               = "female_leggings_fit_wave";

            //FemaleFit_Generic_Ebony_attShoes.Name                         = "shoes0";
            //FemaleFit_Generic_Ebony_attShoes.Matrix                       = "-1";
            //FemaleFit_Generic_Ebony_attShoes.Material                     = @"characters\female\apparel\shoes_black.xmf";
            //FemaleFit_Generic_Ebony_attShoes.Mesh                         = @"characters\female\apparel\fit_generic_shoes.xmd";
            //FemaleFit_Generic_Ebony_attShoes.Size                         = "0.025|0.025|0.025";
            //FemaleFit_Generic_Ebony_attShoes.Vertex                       = "spine2";
            //FemaleFit_Generic_Ebony_attShoes.Orientation                  = "-4.371139E-08|0|-1|0|0|1|0|0|1|0|-4.371139E-08|0|0|0|0|1";
            //FemaleFit_Generic_Ebony_attShoes.Offset                       = "0|0|0";
            //FemaleFit_Generic_Ebony_attShoes.Animation_Idle               = @"female_shoes_fit_idle";
            //FemaleFit_Generic_Ebony_attShoes.Animation_Walk               = @"female_shoes_fit_walk";
            //FemaleFit_Generic_Ebony_attShoes.Animation_Sit                = @"female_shoes_fit_sit";
            //FemaleFit_Generic_Ebony_attShoes.Animation_SitIdle            = @"female_shoes_fit_sidle";
            //FemaleFit_Generic_Ebony_attShoes.Animation_Stand              = @"female_shoes_fit_stand";
            //FemaleFit_Generic_Ebony_attShoes.Animation_Wave               = @"female_shoes_fit_wave";
            #endregion older values

            #endregion

            return FemaleFit_Generic_Ebony_00;
        }


        #region From Server
        public static AttachmentInfoDesc GetFaceAttachment(string userId, string gender, string bodyType, string faceType)
        {
            string offset = "0|-13.3|7.8"; //"0|0|0";
            string size = string.Format("{0}|{0}|{0}", avatarSize.ToString("###.##"));

            switch (gender.ToLower())
            {
                // ------------------- Offset ------------------------------------------- Status -----------
                #region Female (5 x Pending)
                case "female":
                    if (bodyType.ToLower().Contains("normal"))
                    {
                        if (faceType.ToLower().Contains("generic"))
                            offset = "0|-13.3|7.8";                                     // TBC
                        if (faceType.ToLower().Contains("round"))
                            offset = "0|-13.3|7.8";                                     // TBC
                    }

                    if (bodyType.ToLower().Contains("fit"))
                    {
                        if (faceType.ToLower().Contains("generic"))
                            offset = "0|-13.3|7.8";                                     // OK - Done
                        if (faceType.ToLower().Contains("round"))
                            offset = "0|-13.3|7.8";                                     // TBC
                    }

                    if (bodyType.ToLower().Contains("fat"))
                    {
                        if (faceType.ToLower().Contains("generic"))
                            offset = "0|-13.3|7.8";                                     // TBC
                        if (faceType.ToLower().Contains("round"))
                            offset = "0|-13.3|7.8";                                     // TBC
                    }

                    break;
                #endregion

                #region Male (8 x Pending)

                case "male":
                    if (bodyType.ToLower().Contains("slim"))
                    {
                        if (faceType.ToLower().Contains("generic"))
                            offset = "0|-15.0|10.4";                                     // TBC
                        if (faceType.ToLower().Contains("round"))
                            offset = "0|-13.3|7.8";                                     // TBC
                    }
                    if (bodyType.ToLower().Contains("normal"))
                    {
                        if (faceType.ToLower().Contains("generic"))
                            offset = "0|-15.0|10.4";                                    // OK - Looks ok
                        if (faceType.ToLower().Contains("round"))
                            offset = "0|-13.3|7.8";                                     // TBC
                    }
                    if (bodyType.ToLower().Contains("fit"))
                    {
                        if (faceType.ToLower().Contains("generic"))
                        {
                            offset = "0|-15.0|10.4";                                    // OK - Looks ok
                            //size = "0.025|0.026|0.025";
                        }
                        if (faceType.ToLower().Contains("round"))
                            offset = "0|-13.3|7.8";                                     // TBC
                    }
                    if (bodyType.ToLower().Contains("fat"))
                    {
                        if (faceType.ToLower().Contains("generic"))
                            offset = "0|-15.0|10.4";                                    // TBC
                        if (faceType.ToLower().Contains("round"))
                            offset = "0|-13.3|7.8";                                     // TBC
                    }
                    break;

                    #endregion
            }

            return new AttachmentInfoDesc()
            {
                Material    =       (gender.ToLower() == "female" ? @"characters\face_female.xmf"
                                                                  //@"characters\face_female_ebony_00.xmf" 
                                                                  : @"characters\face_male_fit_00.xmf"),
             
                Orientation = "-4.371139E-08|0|-1|0|0|1|0|0|1|0|-4.371139E-08|0|0|0|0|1",

                

                Name = string.Format(@"{0}_face_{1}_{2}", gender, bodyType, faceType),
                Matrix = "5",
                Vertex = "head",
                Mesh = string.Format(@"characters\{0}\face_{1}_{2}.obj", gender, bodyType, faceType),
                Size = size,
                Offset = offset,                                                                                                     // Note Changes per Body Mesh
                Animation_Idle = string.Format(@"{0}_{1}_{2}_idle", gender, bodyType, faceType),
                Animation_Walk = string.Format(@"{0}_{1}_{2}_walk", gender, bodyType, faceType),
                Animation_Sit = string.Format(@"{0}_{1}_{2}_sit", gender, bodyType, faceType),
                Animation_SitIdle = string.Format(@"{0}_{1}_{2}_sidle", gender, bodyType, faceType),
                Animation_Stand = string.Format(@"{0}_{1}_{2}_stand", gender, bodyType, faceType),
                Animation_Wave = string.Format(@"{0}_{1}_{2}_wave", gender, bodyType, faceType)
            };
        }
        public static AttachmentInfoDesc GetHairAttachment(string gender, string hairType, string hairColor)
        {
            // TODO:: Perhaps should query against body type as well
            string offset = "0|0|0";
            
            switch (gender.ToLower())
            {
                // ------------------- Offset ------------------------------------------- Status -----------
                #region Female (All Done)
                case "female":
                    if (hairType.ToLower().Contains("short"))
                        //offset = "0.35|-0.25|0.35";                                     // OK - Done
                        offset = "0|2.35|-1.25";                                     // OK - Done
                    if (hairType.ToLower().Contains("medium"))
                        //offset = "0.5|-0.4|0";                                          // OK - Done
                        offset = "0|0.55|1.0";                                     // OK - Done
                    if (hairType.ToLower().Contains("long"))
                        offset = "0|2|-1";                                              // OK - Done
                    if (hairType.ToLower().Contains("tail"))
                        offset = "-2.5|0|-3.5";                                         // OK - Done
                    break;
                #endregion

                #region Male (3 x Pending)
                case "male":
                    if (hairType.ToLower().Contains("short"))
                        offset = "0|0|2";                                               // OK - Looks fine
                    if (hairType.ToLower().Contains("medium"))
                        offset = "0|0|2";                                               // OK - Looks fine
                    if (hairType.ToLower().Contains("tail"))
                        offset = "0|0|0";                                               // OK - Looks Good
                    break;
                    #endregion
            }

            return new AttachmentInfoDesc()
            {
                Name = string.Format(@"{0}_hairstyle_{1}", gender, hairType),
                Matrix = "5",
                Vertex = "head",
                Mesh = string.Format(@"characters\{0}\hairstyle_{1}.obj", gender, hairType),
                Material = string.Format(@"characters\{0}\hairstyle_{1}_{2}.xmf", gender, hairType, hairColor),

                //Orientation = (gender.ToLower() == "female" ? "-4.371139E-08|0|-1|0|0|1|0|0|1|0|-4.371139E-08|0|0|0|0|1" 
                //                                            : "1|0|0|0|0|1|0|0|0|0|1|0|0|0|0|1"),

                Orientation = "-4.371139E-08|0|-1|0|0|1|0|0|1|0|-4.371139E-08|0|0|0|0|1",
                //Orientation = "1|0|0|0|0|1|0|0|0|0|1|0|0|0|0|1",

                Size = string.Format("{0}|{0}|{0}", avatarSize.ToString("###.##")),
                Offset = offset                                                                                                          // Note: Possibly Changes per Body Mesh
            };
        }
        public static AttachmentInfoDesc GetShirtAttachment(string gender, string bodyType, string faceType, string tshirtColor)
        {
            return new AttachmentInfoDesc()
            {
                Name = string.Format(@"{0}_tshirt_{1}", gender, bodyType),
                Matrix = "-1",
                Mesh = string.Format(@"characters\{0}\apparel\{1}_{2}_tshirt.xmd", gender, bodyType, faceType),                  // Each body mesh has its own t-shirt mesh
                //Material = string.Format(@"characters\{0}\apparel\{1}_tshirt_{2}.xmf", gender, bodyType, tshirtColor),     // T-shirts for each body mesh have their own material                  
                Material = string.Format(@"characters\{0}\apparel\tshirt_{2}.xmf", gender, bodyType, tshirtColor),     // T-shirts for each body mesh have their own material                  
                Orientation = "-4.371139E-08|0|-1|0|0|1|0|0|1|0|-4.371139E-08|0|0|0|0|1",
                Size = string.Format("{0}|{0}|{0}", avatarSize.ToString("###.##")),
                Offset = "0|0|0",
                Animation_Idle = string.Format(@"{0}_{1}_{2}_tshirt_idle", gender, bodyType, faceType),
                Animation_Walk = string.Format(@"{0}_{1}_{2}_tshirt_walk", gender, bodyType, faceType),
                //Animation_Sit       = string.Format(@"{0}_{1}_{2}_tshirt_sit",                              gender, bodyType, faceType),
                //Animation_SitIdle   = string.Format(@"{0}_{1}_{2}_tshirt_sidle",                            gender, bodyType, faceType),
                //Animation_Stand     = string.Format(@"{0}_{1}_{2}_tshirt_stand",                            gender, bodyType, faceType),
                //Animation_Wave      = string.Format(@"{0}_{1}_{2}_tshirt_wave",                             gender, bodyType, faceType),
            };
        }
        public static AttachmentInfoDesc GetLeggingsAttachment(string gender, string bodyType, string faceType, string leggingsColor)
        {
            return new AttachmentInfoDesc()
            {
                Name = string.Format(@"{0}_leggings_{1}", gender, bodyType),
                Matrix = "-1",
                Mesh = string.Format(@"characters\{0}\apparel\{1}_{2}_leggings.xmd", gender, bodyType, faceType),                  // Each body mesh has its own leggings mesh
                //Material = string.Format(@"characters\{0}\apparel\{1}_leggings_{2}.xmf", gender, bodyType, leggingsColor),   // Leggings for each body mesh have their own material
                Material = string.Format(@"characters\{0}\apparel\leggings_{2}.xmf", gender, bodyType, leggingsColor),   // Leggings for each body mesh have their own material
                Orientation = "-4.371139E-08|0|-1|0|0|1|0|0|1|0|-4.371139E-08|0|0|0|0|1",
                Size = string.Format("{0}|{0}|{0}", avatarSize.ToString("###.##")),
                Offset = "0|0|0",
                Animation_Idle = string.Format(@"{0}_{1}_{2}_leggings_idle", gender, bodyType, faceType),
                Animation_Walk = string.Format(@"{0}_{1}_{2}_leggings_walk", gender, bodyType, faceType),
                //Animation_Sit       = string.Format(@"{0}_{1}_{2}_leggings_sit",                            gender, bodyType, faceType),
                //Animation_SitIdle   = string.Format(@"{0}_{1}_{2}_leggings_sidle",                          gender, bodyType, faceType),
                //Animation_Stand     = string.Format(@"{0}_{1}_{2}_leggings_stand",                          gender, bodyType, faceType),
                //Animation_Wave      = string.Format(@"{0}_{1}_{2}_leggings_wave",                           gender, bodyType, faceType),
            };
        }
        public static AttachmentInfoDesc GetShoesAttachment(string gender, string bodyType, string faceType, string shoesColor)
        {
            return new AttachmentInfoDesc()
            {
                Name = string.Format(@"{0}_shoes_{1}", gender, shoesColor),
                Matrix = "-1",
                Mesh = string.Format(@"characters\{0}\apparel\{1}_{2}_shoes.xmd", gender, bodyType, faceType),                  // Each body mesh has its own shoes mesh
                Material = string.Format(@"characters\{0}\apparel\shoes_{1}.xmf", gender, shoesColor),                // Materials common for all body types
                Orientation = "-4.371139E-08|0|-1|0|0|1|0|0|1|0|-4.371139E-08|0|0|0|0|1",
                Size = string.Format("{0}|{0}|{0}", avatarSize.ToString("###.##")),
                Offset = "0|0|0",
                Animation_Idle = string.Format(@"{0}_{1}_{2}_shoes_idle", gender, bodyType, faceType),
                Animation_Walk = string.Format(@"{0}_{1}_{2}_shoes_walk", gender, bodyType, faceType),
                //Animation_Sit       = string.Format(@"{0}_{1}_{2}_shoes_sit",                               gender, bodyType, faceType),
                //Animation_SitIdle   = string.Format(@"{0}_{1}_{2}_shoes_sidle",                             gender, bodyType, faceType),
                //Animation_Stand     = string.Format(@"{0}_{1}_{2}_shoes_stand",                             gender, bodyType, faceType),
                //Animation_Wave      = string.Format(@"{0}_{1}_{2}_shoes_walk",                              gender, bodyType, faceType)
            };
        }

        public static string GetBodyMeshFile(string gender, string faceType, float weight)
        {
            string bodyType = string.Empty;

            switch (gender.ToLower())
            {
                case "male":
                    //if (weight <= 70f) bodyType = "slim";     // NOTE:: Disabled for now
                    if (weight <= 80f) bodyType = "normal";
                    if (weight <= 95f) bodyType = "fit";
                    if (weight > 95f) bodyType = "fat";
                    break;
                case "female":
                    if (weight <= 55f) bodyType = "normal";
                    if (weight <= 70f) bodyType = "fit";
                    if (weight > 70f) bodyType = "fat";
                    break;
            }

            return bodyType;
        }
        public static string GetBodyMaterialFile(string gender, string faceType, float weight)
        {
            string body = string.Empty;

            switch (gender.ToLower())
            {
                case "male":
                    //if (weight <= 70f) body = "slim";
                    if (weight <= 80f) body = "normal";
                    if (weight <= 95f) body = "fit";
                    if (weight > 95f) body = "fat";
                    break;
                case "female":
                    if (weight <= 55f) body = "normal";
                    if (weight <= 70f) body = "fit";
                    if (weight > 70f) body = "fat";
                    break;
            }

            return string.Format(@"characters\{0}\{1}_{2}.xmf", gender, body, faceType);          // TODO:: Should switch to dynamic material after skin color detection from face pic
        }
        #endregion

        /// <summary>
        /// Builds an avatar configuration instance for mixamo 2014-2019 AnimatedModel(s).
        /// Applies a fixed transformation for Size 0.025, for Orientation
        /// -4.371139e-08 0 -1             0
        /// 0             1  0             0
        /// 1             0 -4.371139e-08  0
        /// </summary>
        /// <param name="m0gender">Available options: male, female</param>
        /// <param name="m0body">Available options: normal, fit, fat</param>
        /// <param name="m0face">Available options: generic, round* (round is not configured, use 'generic' by default)</param>
        /// <param name="m0hairtype">Available options: short, medium, tail (for male) and short, medium, long, tail (for female)</param>
        /// <param name="m0haircolor">Available options: black, blonde, brown (for male) and black, blonde, brown, red (for female)</param>
        /// <param name="m0shirtcolor">Available options: blue, brown, red multicolor (for male) and black, blue, green, red, white (for female)</param>
        /// <param name="m0leggingscolor">Available options: and black, blue, green, red, white (for female)</param>
        /// <param name="m0shoescolor">Available options: black, blue, brown, green (for male) and black, blue, green, red, white (for female)</param>
        /// <returns></returns>
        public static AvatarInfoDesc GetAvatar(string m0gender, string m0body, string m0face, string m0hairtype, string m0haircolor, string m0shirtcolor, string m0leggingscolor, string m0shoescolor)
        {

            #region Attachments
            #region Face Attachment
            AttachmentInfoDesc avatar_Generic_0_attFace = new AttachmentInfoDesc();
            avatar_Generic_0_attFace = GetFaceAttachment("local", m0gender, m0body, m0face);
            #endregion

            #region Hair Attachment
            AttachmentInfoDesc avatar_Generic_0_attHair = new AttachmentInfoDesc();
            avatar_Generic_0_attHair = GetHairAttachment(m0gender, m0hairtype, m0haircolor);
            #endregion

            #region Shirt Attachment
            AttachmentInfoDesc avatar_Generic_0_attShirt = new AttachmentInfoDesc();
            avatar_Generic_0_attShirt = GetShirtAttachment(m0gender, m0body, m0face, m0shirtcolor);
            #endregion

            #region Pants Attachment
            AttachmentInfoDesc avatar_Generic_0_attPants = new AttachmentInfoDesc();
            avatar_Generic_0_attPants = GetLeggingsAttachment(m0gender, m0body, m0face, m0leggingscolor);
            #endregion

            #region Shoes Attachment
            AttachmentInfoDesc avatar_Generic_0_attShoes = new AttachmentInfoDesc();
            avatar_Generic_0_attShoes = GetShoesAttachment(m0gender, m0body, m0face, m0shoescolor);
            #endregion
            #endregion

            AvatarInfoDesc avatar = new AvatarInfoDesc(new Guid().ToString(),
                                                m0gender,
                                                string.Format(@"{0}", m0body),
                                                @"generic",
                                                @"Avatar",
                                                string.Format(@"characters\male\body_{0}_generic.xmd", m0body),                                   // Body is ALWAYS .xmd
                                                string.Format(@"characters\male\{0}_generic.xmf", m0body),
                                                string.Format("{0}|{0}|{0}", avatarSize.ToString("###.##")),                                                                             // Hardcoded value -> For 2017-2019 Mixamo Characters
                                                @"-4.371139E-08|0|-1|0|0|1|0|0|1|0|-4.371139E-08|0|0|0|0|1",                                      // Hardcoded value -> For 2017-2019 Mixamo Characters (Z-Flip at Export 3DStudio Max)
                                                @"0|0|0",                                                                                         // Hardcoded value -> Todo: Represents world spawn position... Needs revise
                                                new List<AttachmentInfoDesc>()
                                                    {                                                                                             // *Array Indices*
                                                        avatar_Generic_0_attFace,                                                                 // 0 is ALWAYS Face
                                                        avatar_Generic_0_attHair,                                                                 // 1 is ALWAYS Hair
                                                        avatar_Generic_0_attShirt,                                                                // 2 is ALWAYS Shirt
                                                        avatar_Generic_0_attPants,                                                                // 3 is ALWAYS Pants
                                                        avatar_Generic_0_attShoes                                                                 // 4 is ALWAYS Shoes     
                                                    });

            return avatar;
        }
    }
}
