﻿/*
 * Copyright (C) 2010 ZXing authors
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *      http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

/*
 * These authors would like to acknowledge the Spanish Ministry of Industry,
 * Tourism and Trade, for the support in the project TSI020301-2008-2
 * "PIRAmIDE: Personalizable Interactions with Resources on AmI-enabled
 * Mobile Dynamic Environments", led by Treelogic
 * ( http://www.treelogic.com/ ):
 *
 *   http://www.piramidepse.com/
 */

using System;
using System.Collections.Generic;

namespace ZXing.OneD.RSS.Expanded.Decoders
{
    /// <summary>
    /// <author>Pablo Orduña, University of Deusto (pablo.orduna@deusto.es)</author>
    /// <author>Eduardo Castillejo, University of Deusto (eduardo.castillejo@deusto.es)</author>
    /// </summary>
    static class FieldParser
    {
        private static readonly Object VARIABLE_LENGTH = new Object();

        // "DIGITS", new Integer(LENGTH)
        //    or
        // "DIGITS", VARIABLE_LENGTH, new Integer(MAX_SIZE)
        private static readonly IDictionary<string, DataLength> TWO_DIGIT_DATA_LENGTH;
        private static readonly IDictionary<string, DataLength> THREE_DIGIT_DATA_LENGTH;
        private static readonly IDictionary<string, DataLength> THREE_DIGIT_PLUS_DIGIT_DATA_LENGTH;
        private static readonly IDictionary<string, DataLength> FOUR_DIGIT_DATA_LENGTH;

        static FieldParser()
        {
            TWO_DIGIT_DATA_LENGTH = new Dictionary<string, DataLength>
                                    {
                                       {"00", DataLength.fixedLength(18)},
                                       {"01", DataLength.fixedLength(14)},
                                       {"02", DataLength.fixedLength(14)},
                                       {"10", DataLength.variableLength(20)},
                                       {"11", DataLength.fixedLength(6)},
                                       {"12", DataLength.fixedLength(6)},
                                       {"13", DataLength.fixedLength(6)},
                                       {"15", DataLength.fixedLength(6)},
                                       {"16", DataLength.fixedLength(6)},
                                       {"17", DataLength.fixedLength(6)},
                                       {"20", DataLength.fixedLength(2)},
                                       {"21", DataLength.variableLength(20)},
                                       {"22", DataLength.variableLength(29)}, // limited to 20 in latest versions of spec
                                       {"30", DataLength.variableLength(8)},
                                       {"37", DataLength.variableLength(8)},
                                       //internal company codes
                                       {"90", DataLength.variableLength(30)},
                                       {"91", DataLength.variableLength(30)},
                                       {"92", DataLength.variableLength(30)},
                                       {"93", DataLength.variableLength(30)},
                                       {"94", DataLength.variableLength(30)},
                                       {"95", DataLength.variableLength(30)},
                                       {"96", DataLength.variableLength(30)},
                                       {"97", DataLength.variableLength(30)},
                                       {"98", DataLength.variableLength(30)},
                                       {"99", DataLength.variableLength(30)}
                                    };
            THREE_DIGIT_DATA_LENGTH = new Dictionary<string, DataLength>
                                      {
                                         // Same format as above
                                         {"235", DataLength.variableLength(28)},
                                         {"240", DataLength.variableLength(30)},
                                         {"241", DataLength.variableLength(30)},
                                         {"242", DataLength.variableLength(6)},
                                         {"243", DataLength.variableLength(20)},
                                         {"250", DataLength.variableLength(30)},
                                         {"251", DataLength.variableLength(30)},
                                         {"253", DataLength.variableLength(30)},
                                         {"254", DataLength.variableLength(20)},
                                         {"255", DataLength.variableLength(25)},

                                         {"400", DataLength.variableLength(30)},
                                         {"401", DataLength.variableLength(30)},
                                         {"402", DataLength.fixedLength(17)},
                                         {"403", DataLength.variableLength(30)},
                                         {"410", DataLength.fixedLength(13)},
                                         {"411", DataLength.fixedLength(13)},
                                         {"412", DataLength.fixedLength(13)},
                                         {"413", DataLength.fixedLength(13)},
                                         {"414", DataLength.fixedLength(13)},
                                         {"415", DataLength.fixedLength(13)},
                                         {"416", DataLength.fixedLength(13)},
                                         {"417", DataLength.fixedLength(13)},
                                         {"420", DataLength.variableLength(20)},
                                         {"421", DataLength.variableLength(15)}, // limited to 12 in latest versions of spec
                                         {"422", DataLength.fixedLength(3)},
                                         {"423", DataLength.variableLength(15)},
                                         {"424", DataLength.fixedLength(3)},
                                         {"425", DataLength.variableLength(15)},
                                         {"426", DataLength.fixedLength(3)},
                                         {"427", DataLength.variableLength(3)},

                                         {"710", DataLength.variableLength(20)},
                                         {"711", DataLength.variableLength(20)},
                                         {"712", DataLength.variableLength(20)},
                                         {"713", DataLength.variableLength(20)},
                                         {"714", DataLength.variableLength(20)},
                                         {"715", DataLength.variableLength(20)},
                                      };

            THREE_DIGIT_PLUS_DIGIT_DATA_LENGTH = new Dictionary<string, DataLength>
                                                 {
                                                    {"310", DataLength.fixedLength(6)},
                                                    {"311", DataLength.fixedLength(6)},
                                                    {"312", DataLength.fixedLength(6)},
                                                    {"313", DataLength.fixedLength(6)},
                                                    {"314", DataLength.fixedLength(6)},
                                                    {"315", DataLength.fixedLength(6)},
                                                    {"316", DataLength.fixedLength(6)},
                                                    {"320", DataLength.fixedLength(6)},
                                                    {"321", DataLength.fixedLength(6)},
                                                    {"322", DataLength.fixedLength(6)},
                                                    {"323", DataLength.fixedLength(6)},
                                                    {"324", DataLength.fixedLength(6)},
                                                    {"325", DataLength.fixedLength(6)},
                                                    {"326", DataLength.fixedLength(6)},
                                                    {"327", DataLength.fixedLength(6)},
                                                    {"328", DataLength.fixedLength(6)},
                                                    {"329", DataLength.fixedLength(6)},
                                                    {"330", DataLength.fixedLength(6)},
                                                    {"331", DataLength.fixedLength(6)},
                                                    {"332", DataLength.fixedLength(6)},
                                                    {"333", DataLength.fixedLength(6)},
                                                    {"334", DataLength.fixedLength(6)},
                                                    {"335", DataLength.fixedLength(6)},
                                                    {"336", DataLength.fixedLength(6)},
                                                    {"337", DataLength.fixedLength(6)},
                                                    {"340", DataLength.fixedLength(6)},
                                                    {"341", DataLength.fixedLength(6)},
                                                    {"342", DataLength.fixedLength(6)},
                                                    {"343", DataLength.fixedLength(6)},
                                                    {"344", DataLength.fixedLength(6)},
                                                    {"345", DataLength.fixedLength(6)},
                                                    {"346", DataLength.fixedLength(6)},
                                                    {"347", DataLength.fixedLength(6)},
                                                    {"348", DataLength.fixedLength(6)},
                                                    {"349", DataLength.fixedLength(6)},
                                                    {"350", DataLength.fixedLength(6)},
                                                    {"351", DataLength.fixedLength(6)},
                                                    {"352", DataLength.fixedLength(6)},
                                                    {"353", DataLength.fixedLength(6)},
                                                    {"354", DataLength.fixedLength(6)},
                                                    {"355", DataLength.fixedLength(6)},
                                                    {"356", DataLength.fixedLength(6)},
                                                    {"357", DataLength.fixedLength(6)},
                                                    {"360", DataLength.fixedLength(6)},
                                                    {"361", DataLength.fixedLength(6)},
                                                    {"362", DataLength.fixedLength(6)},
                                                    {"363", DataLength.fixedLength(6)},
                                                    {"364", DataLength.fixedLength(6)},
                                                    {"365", DataLength.fixedLength(6)},
                                                    {"366", DataLength.fixedLength(6)},
                                                    {"367", DataLength.fixedLength(6)},
                                                    {"368", DataLength.fixedLength(6)},
                                                    {"369", DataLength.fixedLength(6)},
                                                    {"390", DataLength.variableLength(15)},
                                                    {"391", DataLength.variableLength(18)},
                                                    {"392", DataLength.variableLength(15)},
                                                    {"393", DataLength.variableLength(18)},
                                                    {"394", DataLength.fixedLength(4)},
                                                    {"395", DataLength.fixedLength(6)},
                                                    {"703", DataLength.variableLength(30)},
                                                    {"723", DataLength.variableLength(30)}
                                                 };

            FOUR_DIGIT_DATA_LENGTH = new Dictionary<string, DataLength>
                                     {
                                        {"4300", DataLength.variableLength(35)},
                                        {"4301", DataLength.variableLength(35)},
                                        {"4302", DataLength.variableLength(70)},
                                        {"4303", DataLength.variableLength(70)},
                                        {"4304", DataLength.variableLength(70)},
                                        {"4305", DataLength.variableLength(70)},
                                        {"4306", DataLength.variableLength(70)},
                                        {"4307", DataLength.fixedLength(2)},
                                        {"4308", DataLength.variableLength(30)},
                                        {"4309", DataLength.fixedLength(20)},
                                        {"4310", DataLength.variableLength(35)},
                                        {"4311", DataLength.variableLength(35)},
                                        {"4312", DataLength.variableLength(70)},
                                        {"4313", DataLength.variableLength(70)},
                                        {"4314", DataLength.variableLength(70)},
                                        {"4315", DataLength.variableLength(70)},
                                        {"4316", DataLength.variableLength(70)},
                                        {"4317", DataLength.fixedLength(2)},
                                        {"4318", DataLength.variableLength(20)},
                                        {"4319", DataLength.variableLength(30)},
                                        {"4320", DataLength.variableLength(35)},
                                        {"4321", DataLength.fixedLength(1)},
                                        {"4322", DataLength.fixedLength(1)},
                                        {"4323", DataLength.fixedLength(1)},
                                        {"4324", DataLength.fixedLength(10)},
                                        {"4325", DataLength.fixedLength(10)},

                                        {"7001", DataLength.fixedLength(13)},
                                        {"7002", DataLength.variableLength(30)},
                                        {"7003", DataLength.fixedLength(10)},
                                        {"7004", DataLength.variableLength(4)},
                                        {"7005", DataLength.variableLength(12)},
                                        {"7006", DataLength.fixedLength(6)},
                                        {"7007", DataLength.variableLength(12)},
                                        {"7008", DataLength.variableLength(3)},
                                        {"7009", DataLength.variableLength(10)},
                                        {"7010", DataLength.variableLength(2)},
                                        {"7011", DataLength.variableLength(10)},
                                        {"7020", DataLength.variableLength(20)},
                                        {"7021", DataLength.variableLength(20)},
                                        {"7022", DataLength.variableLength(20)},
                                        {"7023", DataLength.variableLength(30)},
                                        {"7040", DataLength.fixedLength(4)},
                                        {"7240", DataLength.variableLength(20)},


                                        {"8001", DataLength.fixedLength(14)},
                                        {"8002", DataLength.variableLength(20)},
                                        {"8003", DataLength.variableLength(30)},
                                        {"8004", DataLength.variableLength(30)},
                                        {"8005", DataLength.fixedLength(6)},
                                        {"8006", DataLength.fixedLength(18)},
                                        {"8007", DataLength.variableLength(34)},
                                        {"8008", DataLength.variableLength(12)},
                                        {"8009", DataLength.variableLength(50)},
                                        {"8010", DataLength.variableLength(30)},
                                        {"8011", DataLength.variableLength(12)},
                                        {"8012", DataLength.variableLength(20)},
                                        {"8013", DataLength.variableLength(25)},
                                        {"8017", DataLength.fixedLength(18)},
                                        {"8018", DataLength.fixedLength(18)},
                                        {"8019", DataLength.variableLength(10)},
                                        {"8020", DataLength.variableLength(25)},
                                        {"8026", DataLength.fixedLength(18)},
                                        {"8100", DataLength.fixedLength(6)},  // removed from latest versions of spec
                                        {"8101", DataLength.fixedLength(10)}, // removed from latest versions of spec
                                        {"8102", DataLength.fixedLength(2)},  // removed from latest versions of spec
                                        {"8110", DataLength.variableLength(70)},
                                        {"8111", DataLength.fixedLength(4)},
                                        {"8112", DataLength.variableLength(70)},
                                        {"8200", DataLength.variableLength(70)},
                                     };
        }

        internal static String parseFieldsInGeneralPurpose(String rawInformation)
        {
            if (String.IsNullOrEmpty(rawInformation))
            {
                return null;
            }

            // Processing 2-digit AIs

            if (rawInformation.Length < 2)
            {
                return null;
            }

            String firstTwoDigits = rawInformation.Substring(0, 2);
            DataLength twoDigitDataLength;
            if (TWO_DIGIT_DATA_LENGTH.TryGetValue(firstTwoDigits, out twoDigitDataLength))
            {
                if (twoDigitDataLength.variable)
                {
                    return processVariableAI(2, twoDigitDataLength.length, rawInformation);
                }
                return processFixedAI(2, twoDigitDataLength.length, rawInformation);
            }

            if (rawInformation.Length < 3)
            {
                return null;
            }

            String firstThreeDigits = rawInformation.Substring(0, 3);
            DataLength threeDigitDataLength;
            if (THREE_DIGIT_DATA_LENGTH.TryGetValue(firstThreeDigits, out threeDigitDataLength))
            {
                if (threeDigitDataLength.variable)
                {
                    return processVariableAI(3, threeDigitDataLength.length, rawInformation);
                }
                return processFixedAI(3, threeDigitDataLength.length, rawInformation);
            }

            if (rawInformation.Length < 4)
            {
                return null;
            }

            DataLength threeDigitPlusDigitDataLength;
            if (THREE_DIGIT_PLUS_DIGIT_DATA_LENGTH.TryGetValue(firstThreeDigits, out threeDigitPlusDigitDataLength))
            {
                if (threeDigitPlusDigitDataLength.variable)
                {
                    return processVariableAI(4, threeDigitPlusDigitDataLength.length, rawInformation);
                }
                return processFixedAI(4, threeDigitPlusDigitDataLength.length, rawInformation);
            }

            String firstFourDigits = rawInformation.Substring(0, 4);
            DataLength firstFourDigitLength;
            if (FOUR_DIGIT_DATA_LENGTH.TryGetValue(firstFourDigits, out firstFourDigitLength))
            {
                if (firstFourDigitLength.variable)
                {
                    return processVariableAI(4, firstFourDigitLength.length, rawInformation);
                }
                return processFixedAI(4, firstFourDigitLength.length, rawInformation);
            }

            return null;
        }

        private static String processFixedAI(int aiSize, int fieldSize, String rawInformation)
        {
            if (rawInformation.Length < aiSize)
            {
                return null;
            }

            String ai = rawInformation.Substring(0, aiSize);

            if (rawInformation.Length < aiSize + fieldSize)
            {
                return null;
            }

            String field = rawInformation.Substring(aiSize, fieldSize);
            String remaining = rawInformation.Substring(aiSize + fieldSize);
            String result = '(' + ai + ')' + field;
            String parsedAI = parseFieldsInGeneralPurpose(remaining);
            return parsedAI == null ? result : result + parsedAI;
        }

        private static String processVariableAI(int aiSize, int variableFieldSize, String rawInformation)
        {
            String ai = rawInformation.Substring(0, aiSize);
            int maxSize = Math.Min(rawInformation.Length, aiSize + variableFieldSize);
            String field = rawInformation.Substring(aiSize, maxSize - aiSize);
            String remaining = rawInformation.Substring(maxSize);
            String result = '(' + ai + ')' + field;
            String parsedAI = parseFieldsInGeneralPurpose(remaining);
            return parsedAI == null ? result : result + parsedAI;
        }
    }

    internal sealed class DataLength
    {
        internal bool variable;
        internal int length;

        private DataLength(bool variable, int length)
        {
            this.variable = variable;
            this.length = length;
        }

        public static DataLength fixedLength(int length)
        {
            return new DataLength(false, length);
        }

        public static DataLength variableLength(int length)
        {
            return new DataLength(true, length);
        }
    }
}