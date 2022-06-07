# LifxLan API Overview



## How to use LifxLan

Pull down the `LifxLan.Core` to your own project and add a reference to it via `Add Project Reference` in visual studio.

Create an instance of the `LifxLan.Core.LifxCore` class. 

To find Lifx devices on the network, we create an event `LifxCore.LifxDeviceFound` then call the method `LifxDeviceFound.FindDevicesOnNetwork()`. This will send a broadcast message across the network which any Lifx device will respond to.

When LifxCore.LifxDeviceFound rasies an event, it passes in `Core.LifxProducts.ProductBase product`. This is the Lifx project that has been found. To work out the exact product this is we can inspect the `product.ProductType` `enum` value. 

> NOTE: At present only the Lifx A19 product is supported, but there is nothing stopping you adding support for more products

The below code is an example of everything discussed above.

``` c#

using System;



public class ConsoleApp
{
    // this will be a lifx A16 blub we find on the local network
    private static Core.LifxProducts.LifxBulb _product;
    
    public static void Main()
    {
  
        // create an instance of the LifxCore
        Core.LifxCore lifxCore = new Core.LifxCore();
        // create an event to listen for lifx products that are found on the network
        lifxCore.LifxDeviceFound += LifxCore_LifxDeviceFound;
        // send a broadcast message out to the network asking lifx products to respons
        // so we know who they are
        lifxCore.FindDevicesOnNetwork();

        // set up a while loop that reads a letter on the keyboard. Different letter
        // do different things to the blub. (not this is just example data, you should
        // make sure you have a blub to talk to before trying to interact with it)
        bool CanContinue = true;
        while(CanContinue)
        {

            // wait for the user to press a letter on the keyboard
            ConsoleKeyInfo keyInfo = Console.ReadKey();
            // see if the letter they pressed corresponds to any of the cases below
            switch(keyInfo.KeyChar)
            {
                // quit the application (break away from the while loop)
                case 'q':
                case 'Q':

                    CanContinue = false;
                    break;

                // change the bulb coloro to red
                case 'r':
                    _product.SetColor(Core.LifxProducts.Colors.ColorsList.Red);
                    break;

                // change the blub color to green
                case 'g':
                    _product.SetColor(Core.LifxProducts.Colors.ColorsList.Green);
                    break;

                // change the bulb color to white
                case 'w':
                    _product.SetColor(Core.LifxProducts.Colors.ColorsList.White);
                    break;

               // turn on or off thebulb
                case 'o':
                    _product.TurnLightOnOrOff(!_product.IsPowerdOn);
                    break;

                
            }

        }
        // just before we quit call the finish method to do some tyding up
        lifxCore.Finish();
    }

    // This will get called when a Known lifx product has been found on the local network
    // It will not get called if we don't konw what product it is. Currently only support Lifx A19
    private static void LifxCore_LifxDeviceFound(Core.LifxProducts.ProductBase product)
    {
        // see whic product it is a cast it to the correct datatype
        switch (product.ProductType)
        {
            case Core.LifxProducts.ProductType.Lifx_A19:

                _product = (Core.LifxProducts.LifxBulb)product;
                _product.RequestBulbColor();

                break;
            case Core.LifxProducts.ProductType.Unknown:
                break;
            default:
                break;
        }
    }
}
```



## Casting ProductBase to the correct data type

All Lifx products inherit from `Core.LifxProducts.ProductBase`. The part of the code that decieds which datatype the product should be can be found at `Core.LifxProducts.ProductsList.CreateProduct(string IpAddress, Packets.LifxPacket lifxPacket, ProductType productType)`. Within this method is a switch statment that creates the correct product class.

Below is a list of supported Lifx Products

At present only the Lifx A19 is supported. See below for how to add more products.

| Product  | Data type to cast too (from ProductBase) |
| -------- | ---------------------------------------- |
| Lifx A16 | Core.LifxProducts.LifxBulb               |



## Adding more LifxProducts

All Lifx Products must inherit `Core.LifxProducts.ProductBase`. If a Product is very similar to the Lifx A19 bulb you may be abel to use the `Core.LifxProducts.LifxBulb` class which inherits `Core.LifxProducts.ProductBase`.  This class is for a blub that can do Full Colour.

First you will need to add the product type to `Core.LifxProducts.ProductType` enum. The value you give it is important and can be found in the Product coulmn from the table found at https://lan.developer.lifx.com/docs/product-registry For example if we want to create an enum for `LIFX Color 650` we would give it a value of 3 because that is the value that Lifx have given it in the above url's table.

Next we need to tell the `Core.LifxProducts.ProductsList.CreateProduct` method about our new lifx product. Within the `CreateProduct` method is a switch statment which we need to add too. Following on from the example we used above. If we want to add the `LIFX Color 650` bulb we would add the following to the switch statment.

``` c#
case ProductType.Lifx_Color_650: // this is the enum we created in Core.LifxProducts.ProductType with a valu eof 3
	product = new LifxBulb(lifxPacket.HeaderPacket.Target, 56700, IpAddress,
                       productType, true, false, new HelperClasses.KelvinRange(2500,9000));
	break;
```

> Note: In the above code the last variable passed in to the LifxBulb constructor is the kelvin range the bulb supports. You can find this value in the second table of the above URL (https://lan.developer.lifx.com/docs/product-registry)