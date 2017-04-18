using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
namespace Telerik.UI.Xaml.Controls.Input.Tests
{
    public class CountriesHelper
    {
        public class Country
        {
            public string CountryName { get; set; }
            public int CountryArea { get; set; }
        }

        public static ObservableCollection<Country> GenerateItemsSourceCountries_BusinessObjectList(int numberOfItems)
        {
            ObservableCollection<Country> result = new ObservableCollection<Country>();
            int itemsLeft = numberOfItems;
            using (StringReader reader = new StringReader(countries))
            {
                // skip the first blank line.
                reader.ReadLine();

                if (numberOfItems > 0 && itemsLeft>=0)
                {
                    string line = string.Empty;
                    while (!string.IsNullOrEmpty(line = reader.ReadLine()) && itemsLeft>0)
                    {
                        result.Add(new Country() { CountryName = line, CountryArea = 100 });
                        itemsLeft--;
                    }
                   
                }
                //ALL Countries
                else
                {
                    string line = string.Empty;
                    while (!string.IsNullOrEmpty(line = reader.ReadLine()))
                    {
                        result.Add(new Country() { CountryName = line, CountryArea = 100 });
                    }
                }
            }
            
            return result=new ObservableCollection<Country>(result.Reverse());
        }

        public static ObservableCollection<string> GenerateItemsSourceCountries()
        {
            ObservableCollection<string> result = new ObservableCollection<string>();
            using (StringReader reader = new StringReader(countries))
            {
                // skip the first blank line.
                reader.ReadLine();

                string line = string.Empty;
                while (!string.IsNullOrEmpty(line = reader.ReadLine()))
                {
                   result.Add(line);
                }
            }

            return result;
        }

        public static ObservableCollection<string> GenerateItemsSourceCustom()
        {
            ObservableCollection<string> result = new ObservableCollection<string>();
            result.Add("Afghanistan");
            result.Add("Burundi");
            result.Add("Colombia");
            result.Add("Dominica");
            result.Add("Estonia");
            result.Add("Germany");

            return result;
        }

        private const string countries = @"
Afghanistan
Albania
Algeria
Andorra
Angola
Antigua & Deps
Argentina
Armenia
Australia
Austria
Azerbaijan
Bahamas
Bahrain
Bangladesh
Barbados
Belarus
Belgium
Belize
Benin
Bhutan
Bolivia
Bosnia Herzegovina
Botswana
Brazil
Brunei
Bulgaria
Burkina
Burundi
Cambodia
Cameroon
Canada
Cape Verde
Central African Rep
Chad
Chile
China
Colombia
Comoros
Congo
Congo {Democratic Rep}
Costa Rica
Croatia
Cuba
Cyprus
Czech Republic
Denmark
Djibouti
Dominica
Dominican Republic
East Timor
Ecuador
Egypt
El Salvador
Equatorial Guinea
Eritrea
Estonia
Ethiopia
Fiji
Finland
France
Gabon
Gambia
Georgia
Germany
Ghana
Greece
Grenada
Guatemala
Guinea
Guinea-Bissau
Guyana
Haiti
Honduras
Hungary
Iceland
India
Indonesia
Iran
Iraq
Ireland {Republic}
Israel
Italy
Ivory Coast
Jamaica
Japan
Jordan
Kazakhstan
Kenya
Kiribati
Korea North
Korea South
Kosovo
Kuwait
Kyrgyzstan
Laos
Latvia
Lebanon
Lesotho
Liberia
Libya
Liechtenstein
Lithuania
Luxembourg
Macedonia
Madagascar
Malawi
Malaysia
Maldives
Mali
Malta
Marshall Islands
Mauritania
Mauritius
Mexico
Micronesia
Moldova
Monaco
Mongolia
Montenegro
Morocco
Mozambique
Myanmar, {Burma}
Namibia
Nauru
Nepal
Netherlands
New Zealand
Nicaragua
Niger
Nigeria
Norway
Oman
Pakistan
Palau
Panama
Papua New Guinea
Paraguay
Peru
Philippines
Poland
Portugal
Qatar
Romania
Russian Federation
Rwanda
St Kitts & Nevis
St Lucia
Saint Vincent & the Grenadines
Samoa
San Marino
Sao Tome & Principe
Saudi Arabia
Senegal
Serbia
Seychelles
Sierra Leone
Singapore
Slovakia
Slovenia
Solomon Islands
Somalia
South Africa
Spain
Sri Lanka
Sudan
Suriname
Swaziland
Sweden
Switzerland
Syria
Taiwan
Tajikistan
Tanzania
Thailand
Togo
Tonga
Trinidad & Tobago
Tunisia
Turkey
Turkmenistan
Tuvalu
Uganda
Ukraine
United Arab Emirates
United Kingdom
United States
Uruguay
Uzbekistan
Vanuatu
Vatican City
Venezuela
Vietnam
Yemen
Zambia
Zimbabwe
rrrrrrr!!!";
    }
}
