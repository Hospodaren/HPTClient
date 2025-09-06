using System.Linq;

namespace HPTClient
{
    public class HPTTrackDistance
    {
        public static int GetDistance(TrackNameEnum raceTrack, TrackNameEnum horseTrack)
        {
            // Hemmabana
            if (horseTrack == raceTrack)
            {
                return 0;
            }

            // Hämta position i arrayen
            var raceTrackDistance = TrackDistanceArray.FirstOrDefault(td => td.TrackName == raceTrack);
            var horseTrackDistance = TrackDistanceArray.FirstOrDefault(td => td.TrackName == horseTrack);
            if (horseTrackDistance != null && raceTrackDistance != null)
            {
                return raceTrackDistance.DistanceArray[horseTrackDistance.PositionInArray];
            }

            // Ingen träff på bana
            return 0;
        }

        public int GetDistance(TrackNameEnum horseTrack)
        {
            // Hemmabana
            if (horseTrack == this.TrackName)
            {
                return 0;
            }

            // Hämta position i arrayen
            var horseTrackDistance = TrackDistanceArray.FirstOrDefault(td => td.TrackName == horseTrack);
            if (horseTrackDistance != null)
            {
                return this.DistanceArray[horseTrackDistance.PositionInArray];
            }

            // Ingen träff på bana
            return 0;
        }

        private static HPTTrackDistance[] trackDistanceArray;
        public static HPTTrackDistance[] TrackDistanceArray
        {
            get
            {
                if (trackDistanceArray == null)
                {
                    trackDistanceArray = CreateTrackDistanceArray();
                }
                return trackDistanceArray;
            }
        }

        public static HPTTrackDistance[] CreateTrackDistanceArray()
        {
            var trackDistanceArray = new HPTTrackDistance[]
            {
                new HPTTrackDistance()
                {
                    TrackName = TrackNameEnum.Arvika,
                    PositionInArray = 0,
                    DistanceArray = new int[] { 0, 240, 543, 1089, 372, 644, 270, 68, 397, 463, 402, 709, 537, 509, 519, 205, 871, 280, 530, 292, 276, 371, 932, 685, 474, 798, 339, 393, 271, 94, 62, 180, 559 }
                },
                new HPTTrackDistance()
                {
                    TrackName = TrackNameEnum.Axevalla,
                    PositionInArray = 1,
                    DistanceArray = new int[] { 240, 0, 606, 1153, 455, 707, 249, 169, 397, 526, 274, 819, 408, 300, 310, 203, 948, 182, 640, 325, 382, 350, 996, 749, 266, 861, 130, 290, 142, 179, 264, 152, 668 }
                },
                new HPTTrackDistance()
                {
                    TrackName = TrackNameEnum.Bergsåker,
                    PositionInArray = 2,
                    DistanceArray = new int[] { 543, 606, 0, 554, 181, 108, 405, 535, 217, 84, 830, 246, 946, 836, 749, 403, 348, 562, 199, 325, 291, 379, 396, 150, 799, 262, 682, 446, 735, 611, 630, 458, 183 }
                },
                new HPTTrackDistance()
                {
                    TrackName = TrackNameEnum.Boden,
                    PositionInArray = 3,
                    DistanceArray = new int[] { 1089, 1153, 554, 0, 727, 484, 951, 1081, 763, 630, 1376, 444, 1492, 1382, 1295, 949, 291, 1108, 633, 872, 837, 925, 158, 406, 1345, 296, 1229, 992, 1281, 1157, 1176, 1004, 591 }
                },
                new HPTTrackDistance()
                {
                    TrackName = TrackNameEnum.Bollnäs,
                    PositionInArray = 4,
                    DistanceArray = new int[] { 372, 455, 181, 727, 0, 282, 291, 351, 103, 101, 679, 428, 794, 684, 635, 252, 523, 424, 258, 141, 112, 265, 571, 324, 648, 436, 531, 332, 584, 427, 445, 307, 277 }
                },
                new HPTTrackDistance()
                {
                    TrackName = TrackNameEnum.Dannero,
                    PositionInArray = 5,
                    DistanceArray = new int[] { 644, 707, 108, 484, 282, 0, 506, 636, 319, 185, 931, 184, 1047, 936, 850, 504, 271, 663, 244, 427, 392, 480, 327, 80, 900, 192, 783, 547, 836, 712, 731, 559, 200 }
                },
                new HPTTrackDistance()
                {
                    TrackName = TrackNameEnum.Eskilstuna,
                    PositionInArray = 6,
                    DistanceArray = new int[] { 270, 249, 405, 951, 291, 506, 0, 200, 196, 325, 473, 635, 588, 455, 365, 104, 746, 177, 567, 175, 245, 116, 794, 547, 419, 660, 325, 134, 379, 276, 294, 101, 582 }
                },
                new HPTTrackDistance()
                {
                    TrackName = TrackNameEnum.Färjestad,
                    PositionInArray = 7,
                    DistanceArray = new int[] { 68, 169, 535, 1081, 351, 636, 200, 0, 327, 456, 388, 704, 523, 439, 448, 134, 866, 209, 524, 221, 270, 301, 925, 679, 404, 791, 269, 323, 257, 80, 98, 109, 553 }
                },
                new HPTTrackDistance()
                {
                    TrackName = TrackNameEnum.Gävle,
                    PositionInArray = 8,
                    DistanceArray = new int[] { 397, 397, 217, 763, 103, 319, 196, 327, 0, 135, 620, 447, 736, 626, 540, 193, 559, 352, 355, 116, 142, 167, 605, 358, 590, 470, 473, 234, 526, 402, 420, 248, 393 }
                },
                new HPTTrackDistance()
                {
                    TrackName = TrackNameEnum.Hagmyren,
                    PositionInArray = 9,
                    DistanceArray = new int[] { 463, 526, 84, 630, 101, 185, 325, 456, 135, 0, 750, 313, 865, 755, 669, 323, 425, 482, 277, 245, 211, 299, 473, 226, 719, 338, 602, 366, 655, 531, 550, 378, 261 }
                },
                new HPTTrackDistance()
                {
                    TrackName = TrackNameEnum.Halmstad,
                    PositionInArray = 10,
                    DistanceArray = new int[] { 402, 274, 830, 1376, 679, 931, 473, 388, 620, 750, 0, 1033, 141, 186, 234, 427, 1171, 283, 854, 549, 606, 504, 1219, 973, 176, 1085, 150, 260, 134, 311, 372, 376, 882 }
                },
                new HPTTrackDistance()
                {
                    TrackName = TrackNameEnum.Hoting,
                    PositionInArray = 11,
                    DistanceArray = new int[] { 709, 819, 246, 444, 428, 184, 635, 704, 447, 313, 1033, 0, 1182, 1090, 979, 680, 169, 852, 196, 573, 504, 609, 372, 219, 1054, 244, 919, 676, 949, 780, 798, 735, 153 }
                },
                new HPTTrackDistance()
                {
                    TrackName = TrackNameEnum.Jägersro,
                    PositionInArray = 12,
                    DistanceArray = new int[] { 537, 408, 946, 1492, 794, 1047, 588, 523, 736, 865, 141, 1182, 0, 155, 281, 542, 1287, 399, 1003, 665, 722, 620, 1335, 1088, 189, 1200, 266, 330, 268, 447, 505, 491, 1031 }
                },

                new HPTTrackDistance()
                {
                    TrackName = TrackNameEnum.Karlshamn,
                    PositionInArray = 13,
                    DistanceArray = new int[] { 509, 300, 836, 1382, 684, 936, 455, 439, 626, 755, 186, 1090, 155, 0, 138, 432, 1177, 280, 909, 555, 612, 502, 1225, 978, 36, 1090, 179, 218, 313, 441, 500, 381, 938 }
                },

                new HPTTrackDistance()
                {
                    TrackName = TrackNameEnum.Kalmar,
                    PositionInArray = 14,
                    DistanceArray = new int[] { 519, 310, 749, 1295, 635, 850, 365, 448, 540, 669, 234, 979, 281, 138, 0, 407, 1091, 238, 907, 520, 586, 412, 1139, 893, 98, 1005, 211, 82, 336, 450, 509, 361, 936 }
                },
                new HPTTrackDistance()
                {
                    TrackName = TrackNameEnum.Lindesberg,
                    PositionInArray = 15,
                    DistanceArray = new int[] { 205, 203, 403, 949, 252, 504, 104, 134, 193, 323, 427, 680, 542, 432, 407, 0, 744, 172, 501, 122, 179, 202, 793, 546, 397, 658, 279, 227, 332, 212, 230, 55, 530 }
                },
                new HPTTrackDistance()
                {
                    TrackName = TrackNameEnum.Lycksele,
                    PositionInArray = 16,
                    DistanceArray = new int[] { 871, 948, 348, 291, 523, 271, 746, 866, 559, 425, 1171, 169, 1287, 1177, 1091, 744, 0, 904, 358, 667, 666, 721, 161, 202, 1141, 137, 1024, 788, 1077, 942, 960, 800, 315 }
                },
                new HPTTrackDistance()
                {
                    TrackName = TrackNameEnum.Mantorp,
                    PositionInArray = 17,
                    DistanceArray = new int[] { 280, 182, 562, 1108, 424, 663, 177, 209, 352, 482, 283, 852, 399, 280, 238, 172, 904, 0, 673, 294, 351, 224, 952, 705, 246, 817, 135, 193, 252, 285, 304, 121, 702 }
                },
                new HPTTrackDistance()
                {
                    TrackName = TrackNameEnum.Oviken,
                    PositionInArray = 18,
                    DistanceArray = new int[] { 530, 640, 199, 633, 258, 244, 567, 524, 355, 277, 854, 196, 1003, 909, 907, 501, 358, 673, 0, 393, 325, 517, 561, 293, 875, 405, 739, 584, 769, 600, 619, 556, 46 }
                },
                new HPTTrackDistance()
                {
                    TrackName = TrackNameEnum.Romme,
                    PositionInArray = 19,
                    DistanceArray = new int[] { 292, 325, 325, 872, 141, 427, 175, 221, 116, 245, 549, 573, 665, 555, 520, 122, 667, 294, 393, 0, 71, 201, 715, 468, 518, 580, 401, 273, 454, 296, 315, 177, 421 }
                },
                new HPTTrackDistance()
                {
                    TrackName = TrackNameEnum.Rättvik,
                    PositionInArray = 20,
                    DistanceArray = new int[] { 276, 382, 291, 837, 112, 392, 245, 270, 142, 211, 606, 504, 722, 612, 586, 179, 666, 351, 325, 71, 0, 270, 681, 434, 576, 546, 458, 341, 512, 346, 365, 234, 353 }
                },
                new HPTTrackDistance()
                {
                    TrackName = TrackNameEnum.Solvalla,
                    PositionInArray = 21,
                    DistanceArray = new int[] { 371, 350, 379, 925, 265, 480, 116, 301, 167, 299, 504, 609, 620, 502, 412, 202, 721, 224, 517, 201, 270, 0, 769, 522, 465, 635, 357, 73, 472, 378, 396, 203, 557 }
                },
                new HPTTrackDistance()
                {
                    TrackName = TrackNameEnum.Skellefteå,
                    PositionInArray = 22,
                    DistanceArray = new int[] { 932, 996, 396, 158, 571, 327, 794, 925, 605, 473, 1219, 372, 1335, 1225, 1139, 793, 161, 952, 561, 715, 681, 769, 0, 250, 1189, 139, 1072, 836, 1125, 1001, 1019, 847, 518 }
                },
                new HPTTrackDistance()
                {
                    TrackName = TrackNameEnum.Solänget,
                    PositionInArray = 23,
                    DistanceArray = new int[] { 685, 749, 150, 406, 324, 80, 547, 679, 358, 226, 973, 219, 1088, 978, 893, 546, 202, 705, 293, 468, 434, 522, 250, 0, 942, 114, 825, 589, 878, 754, 773, 601, 249 }
                },
                new HPTTrackDistance()
                {
                    TrackName = TrackNameEnum.Tingsryd,
                    PositionInArray = 24,
                    DistanceArray = new int[] { 474, 266, 799, 1345, 648, 900, 419, 404, 590, 719, 176, 1054, 189, 36, 98, 397, 1141, 246, 875, 518, 576, 465, 1189, 942, 0, 1054, 153, 177, 280, 405, 464, 345, 902 }
                },
                new HPTTrackDistance()
                {
                    TrackName = TrackNameEnum.Umåker,
                    PositionInArray = 25,
                    DistanceArray = new int[] { 798, 861, 262, 296, 436, 192, 660, 791, 470, 338, 1085, 244, 1200, 1090, 1005, 658, 137, 817, 405, 580, 546, 635, 139, 114, 1054, 0, 937, 701, 990, 866, 885, 713, 361 }
                },
                new HPTTrackDistance()
                {
                    TrackName = TrackNameEnum.Vaggeryd,
                    PositionInArray = 26,
                    DistanceArray = new int[] { 339, 130, 682, 1229, 531, 783, 325, 269, 473, 602, 150, 919, 266, 179, 211, 279, 1024, 135, 739, 401, 458, 357, 1072, 825, 153, 937, 0, 178, 172, 271, 329, 228, 768 }
                },
                new HPTTrackDistance()
                {
                    TrackName = TrackNameEnum.Visby,
                    PositionInArray = 27,
                    DistanceArray = new int[] { 393, 290, 446, 992, 332, 547, 134, 323, 234, 366, 260, 676, 330, 218, 82, 227, 788, 193, 584, 273, 341, 73, 836, 589, 177, 701, 178, 0, 335, 399, 417, 224, 623 }
                },
                new HPTTrackDistance()
                {
                    TrackName = TrackNameEnum.Åby,
                    PositionInArray = 28,
                    DistanceArray = new int[] { 271, 142, 735, 1281, 584, 836, 379, 257, 526, 655, 134, 949, 268, 313, 336, 332, 1077, 252, 769, 454, 512, 472, 1125, 878, 280, 990, 172, 335, 0, 181, 240, 281, 798 }
                },
                new HPTTrackDistance()
                {
                    TrackName = TrackNameEnum.Åmål,
                    PositionInArray = 29,
                    DistanceArray = new int[] { 94, 179, 611, 1157, 427, 712, 276, 80, 402, 531, 311, 780, 447, 441, 450, 212, 942, 285, 600, 296, 346, 378, 1001, 754, 405, 866, 271, 399, 181, 0, 62, 185, 629 }
                },
                new HPTTrackDistance()
                {
                    TrackName = TrackNameEnum.Årjäng,
                    PositionInArray = 30,
                    DistanceArray = new int[] { 62, 264, 630, 1176, 445, 731, 294, 98, 420, 550, 372, 798, 505, 500, 509, 230, 960, 304, 619, 315, 365, 396, 1019, 773, 464, 885, 329, 417, 240, 62, 0, 204, 648 }
                },
                new HPTTrackDistance()
                {
                    TrackName = TrackNameEnum.Örebro,
                    PositionInArray = 31,
                    DistanceArray = new int[] { 180, 152, 458, 1004, 307, 559, 101, 109, 248, 378, 376, 735, 491, 381, 361, 55, 800, 121, 556, 177, 234, 203, 847, 601, 345, 713, 228, 224, 281, 185, 204, 0, 584 }
                },
                new HPTTrackDistance()
                {
                    TrackName = TrackNameEnum.Östersund,
                    PositionInArray = 32,
                    DistanceArray = new int[] { 559, 668, 183, 591, 277, 200, 582, 553, 393, 261, 882, 153, 1031, 938, 936, 530, 315, 702, 46, 421, 353, 557, 518, 249, 902, 361, 768, 623, 798, 629, 648, 584, 0 }
                }
            };

            return trackDistanceArray;
        }

        //public static HPTTrackDistance[] CreateTrackDistanceArray()
        //{
        //    var trackDistanceArray = new HPTTrackDistance[]
        //    {
        //        new HPTTrackDistance()
        //        {
        //            TrackName = TrackNameEnum.Arvika,
        //            PositionInArray = 0,
        //            DistanceArray = new int[] { 0,24,51,106,37,61,27,7,37,44,39,69,52,49,20,84,27,51,27,27,90,37,66,77,34,48,26,8,5,18,54 }
        //        },
        //        new HPTTrackDistance()
        //        {
        //            TrackName = TrackNameEnum.Axevalla,
        //            PositionInArray = 1,
        //            DistanceArray = new int[] { 24,0,58,113,45,68,24,16,38,50,22,80,36,30,19,91,18,62,31,34,98,34,73,84,13,37,14,17,22,15,65 }
        //        },
        //        new HPTTrackDistance()
        //        {
        //            TrackName = TrackNameEnum.Bergsåker,
        //            PositionInArray = 2,
        //            DistanceArray = new int[] { 51,58,0,55,16,9,40,51,22,9,79,23,91,73,39,33,55,20,30,27,39,38,15,26,66,58,72,58,57,43,19 }
        //        },
        //        new HPTTrackDistance()
        //        {
        //            TrackName = TrackNameEnum.Boden,
        //            PositionInArray = 3,
        //            DistanceArray = new int[] { 106,113,55,0,71,48,95,106,77,63,133,41,146,128,94,26,110,61,85,82,16,93,40,29,121,113,127,112,112,98,57 }
        //        },
        //        new HPTTrackDistance()
        //        {
        //            TrackName = TrackNameEnum.Bollnäs,
        //            PositionInArray = 4,
        //            DistanceArray = new int[] { 37,45,16,71,0,26,27,34,10,74,65,39,78,60,25,50,41,25,14,12,56,26,31,42,53,47,58,42,43,30,27 }
        //        },
        //        new HPTTrackDistance()
        //        {
        //            TrackName = TrackNameEnum.Dannero,
        //            PositionInArray = 5,
        //            DistanceArray = new int[] { 61,68,9,48,26,0,49,60,31,18,88,17,100,83,48,26,65,21,40,37,32,47,8,19,76,68,82,67,66,53,18 }
        //        },
        //        new HPTTrackDistance()
        //        {
        //            TrackName = TrackNameEnum.Eskilstuna,
        //            PositionInArray = 6,
        //            DistanceArray = new int[] { 27,24,40,95,27,49,0,19,18,19,42,63,54,34,97,73,16,50,16,22,79,12,55,66,29,27,37,26,29,11,53 }
        //        },
        //        new HPTTrackDistance()
        //        {
        //            TrackName = TrackNameEnum.Färjestad,
        //            PositionInArray = 7,
        //            DistanceArray = new int[] { 7,16,51,106,34,60,19,0,32,43,38,69,51,42,13,84,20,51,21,24,90,30,65,68,27,41,26,8,10,11,55 }
        //        },
        //        new HPTTrackDistance()
        //        {
        //            TrackName = TrackNameEnum.Gävle,
        //            PositionInArray = 8,
        //            DistanceArray = new int[] { 37,38,22,77,10,31,18,32,0,13,58,44,70,52,19,54,34,35,11,13,61,17,36,47,46,37,52,39,41,23,36 }
        //        },
        //        new HPTTrackDistance()
        //        {
        //            TrackName = TrackNameEnum.Hagmyren,
        //            PositionInArray = 9,
        //            DistanceArray = new int[] { 44,50,9,63,74,18,19,43,13,0,70,31,82,65,31,42,47,25,22,19,48,30,23,34,58,50,64,49,50,35,24 }
        //        },
        //        new HPTTrackDistance()
        //        {
        //            TrackName = TrackNameEnum.Halmstad,
        //            PositionInArray = 10,
        //            DistanceArray = new int[] { 39,22,79,133,65,88,42,38,58,70,0,101,13,24,40,112,27,84,51,56,118,49,93,105,14,38,13,31,36,35,87 }
        //        },
        //        new HPTTrackDistance()
        //        {
        //            TrackName = TrackNameEnum.Hoting,
        //            PositionInArray = 11,
        //            DistanceArray = new int[] { 69,80,23,41,39,17,63,69,44,31,101,0,113,96,62,16,78,19,54,47,31,60,18,24,89,81,93,76,75,66,15 }
        //        },
        //        new HPTTrackDistance()
        //        {
        //            TrackName = TrackNameEnum.Jägersro,
        //            PositionInArray = 12,
        //            DistanceArray = new int[] { 52,36,91,146,78,100,54,51,70,82,13,113,0,27,51,123,38,96,63,68,130,60,105,116,24,44,26,44,48,47,99 }
        //        },
        //        new HPTTrackDistance()
        //        {
        //            TrackName = TrackNameEnum.Kalmar,
        //            PositionInArray = 13,
        //            DistanceArray = new int[] { 49,30,73,128,60,83,34,42,52,65,24,96,27,0,37,106,23,83,49,54,113,39,88,99,19,18,34,44,49,33,86 }
        //        },
        //        new HPTTrackDistance()
        //        {
        //            TrackName = TrackNameEnum.Lindesberg,
        //            PositionInArray = 14,
        //            DistanceArray = new int[] { 20,19,39,94,25,48,97,13,19,31,40,62,51,37,0,70,16,46,13,17,78,18,54,65,27,36,33,20,23,4,49 }
        //        },
        //        new HPTTrackDistance()
        //        {
        //            TrackName = TrackNameEnum.Lycksele,
        //            PositionInArray = 15,
        //            DistanceArray = new int[] { 84,91,33,26,50,26,73,84,54,42,112,16,123,106,70,0,88,35,64,60,15,71,18,13,99,91,105,91,90,76,31 }
        //        },
        //        new HPTTrackDistance()
        //        {
        //            TrackName = TrackNameEnum.Mantorp,
        //            PositionInArray = 16,
        //            DistanceArray = new int[] { 27,18,55,110,41,65,16,20,34,47,27,78,38,23,16,88,0,62,29,33,95,22,70,81,14,23,25,27,29,12,65 }
        //        },
        //        new HPTTrackDistance()
        //        {
        //            TrackName = TrackNameEnum.Oviken,
        //            PositionInArray = 17,
        //            DistanceArray = new int[] { 51,62,20,61,25,21,50,51,35,25,84,19,96,83,46,35,62,0,35,29,51,51,27,38,73,72,76,58,57,50,4 }
        //        },
        //        new HPTTrackDistance()
        //        {
        //            TrackName = TrackNameEnum.Romme,
        //            PositionInArray = 18,
        //            DistanceArray = new int[] { 27,31,30,85,14,40,16,21,11,22,51,54,63,49,13,64,29,35,0,6,69,21,47,58,40,42,45,28,30,16,38 }
        //        },
        //        new HPTTrackDistance()
        //        {
        //            TrackName = TrackNameEnum.Rättvik,
        //            PositionInArray = 19,
        //            DistanceArray = new int[] { 27,34,27,82,12,37,22,24,13,19,56,47,68,54,17,60,33,29,6,0,67,26,42,53,44,47,48,31,33,21,32 }
        //        },
        //        new HPTTrackDistance()
        //        {
        //            TrackName = TrackNameEnum.Skellefteå,
        //            PositionInArray = 20,
        //            DistanceArray = new int[] { 90,98,39,16,56,32,79,90,61,48,118,31,130,113,78,15,95,51,69,67,0,77,24,13,106,97,111,96,96,82,46 }
        //        },
        //        new HPTTrackDistance()
        //        {
        //            TrackName = TrackNameEnum.Solvalla,
        //            PositionInArray = 21,
        //            DistanceArray = new int[] { 37,34,38,93,26,47,12,30,17,30,49,60,60,39,18,71,22,51,77,26,21,0,53,64,36,21,48,37,40,19,53 }
        //        },
        //        new HPTTrackDistance()
        //        {
        //            TrackName = TrackNameEnum.Solänget,
        //            PositionInArray = 22,
        //            DistanceArray = new int[] { 66,73,15,40,31,8,55,65,36,23,93,18,105,88,54,18,70,27,24,42,47,53,0,11,82,73,87,72,71,58,25 }
        //        },
        //        new HPTTrackDistance()
        //        {
        //            TrackName = TrackNameEnum.Umeå,
        //            PositionInArray = 23,
        //            DistanceArray = new int[] { 77,84,26,29,42,19,66,68,47,34,105,24,116,99,65,13,81,38,13,53,58,64,11,0,92,84,98,83,83,69,36 }
        //        },
        //        new HPTTrackDistance()
        //        {
        //            TrackName = TrackNameEnum.Vaggeryd,
        //            PositionInArray = 24,
        //            DistanceArray = new int[] { 34,13,66,121,53,76,29,27,46,58,14,89,24,19,27,99,14,73,106,44,40,36,82,92,0,28,16,27,32,23,76 }
        //        },
        //        new HPTTrackDistance()
        //        {
        //            TrackName = TrackNameEnum.Visby,
        //            PositionInArray = 25,
        //            DistanceArray = new int[] { 48,37,58,113,47,68,27,41,37,50,38,81,44,18,36,91,23,72,97,47,42,21,73,84,28,0,42,48,51,31,73 }
        //        },
        //        new HPTTrackDistance()
        //        {
        //            TrackName = TrackNameEnum.Åby,
        //            PositionInArray = 26,
        //            DistanceArray = new int[] { 26,14,72,127,58,82,37,26,52,64,13,93,26,34,33,105,25,76,111,48,45,48,87,98,16,42,0,18,23,29,79 }
        //        },
        //        new HPTTrackDistance()
        //        {
        //            TrackName = TrackNameEnum.Åmål,
        //            PositionInArray = 27,
        //            DistanceArray = new int[] { 8,17,58,112,42,67,26,8,39,49,31,76,44,44,20,91,27,58,97,31,28,37,72,83,27,48,18,0,6,18,61 }
        //        },
        //        new HPTTrackDistance()
        //        {
        //            TrackName = TrackNameEnum.Årjäng,
        //            PositionInArray = 28,
        //            DistanceArray = new int[] { 53,22,57,112,43,66,29,10,41,50,36,75,48,49,23,90,29,57,96,33,30,40,71,83,32,51,23,6,0,21,60 }
        //        },
        //        new HPTTrackDistance()
        //        {
        //            TrackName = TrackNameEnum.Örebro,
        //            PositionInArray = 29,
        //            DistanceArray = new int[] { 18,15,43,98,30,53,11,11,23,35,35,66,47,33,4,76,12,50,16,21,16,19,58,69,23,31,29,18,21,0,53 }
        //        },
        //        new HPTTrackDistance()
        //        {
        //            TrackName = TrackNameEnum.Östersund,
        //            PositionInArray = 30,
        //            DistanceArray = new int[] { 54,65,19,57,27,18,53,55,36,24,87,15,99,86,49,31,65,4,46,32,38,53,25,36,76,73,79,61,60,53,0 }
        //        }
        //    };

        //    return trackDistanceArray;
        //}

        public TrackNameEnum TrackName { get; set; }

        public int[] DistanceArray { get; set; }

        public int PositionInArray { get; set; }
    }
}
