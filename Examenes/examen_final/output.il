.assembly 'suduma' {}

.class public 'final_exam' extends ['mscorlib']'System'.'Object' {
	.method public static void 'start'() {
		.entrypoint
		ldc.i4.1
		ldc.i4.1
		dup
		add
		dup
		add
		ldc.i4.2
		call int32 ['mscorlib']'System'.'Math'::'Max'(int32, int32)
		ldc.i4.3
		dup
		add
		call int32 ['mscorlib']'System'.'Math'::'Max'(int32, int32)
		add
		ldc.i4.2
		add
		ldc.i4.2
		dup
		add
		call int32 ['mscorlib']'System'.'Math'::'Max'(int32, int32)
		call void ['mscorlib']'System'.'Console'::'WriteLine'(int32)
		ret
	}
}
