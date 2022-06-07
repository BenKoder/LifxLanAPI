// See https://aka.ms/new-console-template for more information


using System;



public class ConsoleApp
{
    private static Core.LifxProducts.LifxBulb _product;
    public static void Main()
    {
  

        Console.WriteLine("Searching for lifx products.");

        Core.LifxCore lifxCore = new Core.LifxCore();
        lifxCore.LifxDeviceFound += LifxCore_LifxDeviceFound;
        lifxCore.FindDevicesOnNetwork();

        Console.WriteLine("Press q to quit");
        Console.WriteLine("Press o to turn bulb on or off");
        Console.WriteLine("Press r to change bulb color to red");
        Console.WriteLine("Press g to vhange bulb color to green");
        Console.WriteLine("Press w to vhange bulb color to white");

        bool CanContinue = true;
        while(CanContinue)
        {

            ConsoleKeyInfo keyInfo = Console.ReadKey();
            switch(keyInfo.KeyChar)
            {
                case 'q':
                case 'Q':

                    CanContinue = false;
                    break;

                case 'r':
                    _product?.SetColor(Core.LifxProducts.Colors.ColorsList.Red);
                    break;

                case 'g':
                    _product?.SetColor(Core.LifxProducts.Colors.ColorsList.Green);
                    break;

                case 'w':
                    _product?.SetColor(Core.LifxProducts.Colors.ColorsList.White);
                    break;

                case 'o':
                    _product?.TurnLightOnOrOff(!_product.IsPowerdOn);
                    break;

                
            }

        }
        
        lifxCore.Finish();
    }

    private static void LifxCore_LifxDeviceFound(Core.LifxProducts.ProductBase product)
    {
        switch (product.ProductType)
        {
            case Core.LifxProducts.ProductType.Lifx_A19:

                _product = (Core.LifxProducts.LifxBulb)product;
                //Core.LifxProducts.Colors.
                //((Core.LifxProducts.LifxBulb)product).SetColor()
                _product.RequestBulbColor();

                Console.WriteLine("A19 bulb found");
                break;

            case Core.LifxProducts.ProductType.Unknown:
                Console.WriteLine("Unknown product found");
                break;

            default:
                Console.WriteLine("Unknown product found");
                break;
        }
    }
}
