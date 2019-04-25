// CIL example program.
//
// To assemble:
//                 ilasm simple_example.il

.assembly 'example' { }

.class public 'Test' extends ['mscorlib']'System'.'Object' {
    .method public static void 'whatever'() {
        .entrypoint
        // ldc.i4 42
        // call void class ['mscorlib']'System'.'Console'::'WriteLine'(int32)

        //add
        // ldc.i4 100
        // ldc.i4 42
        // add
        // call void class ['mscorlib']'System'.'Console'::'WriteLine'(int32)

        //sub
        // ldc.i4 100
        // ldc.i4 42
        // sub
        // call void class ['mscorlib']'System'.'Console'::'WriteLine'(int32)
        // ldc.i4 1 //runtime error , pop solves it     
        // pop



        //(-(10+50-12)*(32%5))^3
        ldc.i4 10
        ldc.i4 50
        ldc.i4 12
        sub
        add
        neg
        ldc.i4 32
        ldc.i4 5
        rem
        mul
        //call void class ['mscorlib']'System'.'Console'::'WriteLine'(int32)

        conv.r8
        ldc.r8 3
        call float64 class ['mscorlib']System.Math::Pow(float64, float64)
        conv.i4
        call void class ['mscorlib']'System'.'Console'::'WriteLine'(int32)
        ret //stack size should be 0


        //wyvern todas las clases regresan algo, 0 by default
    }
}
