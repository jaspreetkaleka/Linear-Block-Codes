# Linear Block Codes

An (n,k) binary linear block code generates a block of 'n' coded bits called codeword symbols <U> from 'k' bits of information called message <m> using a generator matrix <G>.

There are 2^n possible values for 'n' bits.  We select 2^k values from these 2^n possibilities to form the codeword <U>, such that each message <m> is uniquely mapped to one of these 2^k codewords. If this mapping is linear then the block code is called a linear block code.


	One Message <m> 	= 	< m1 m2 ... mk >
	[ 1 x k ]

	Generator <G> 		= 	< v11 v12 ... v1n  >
	[ k x n ]			< v21 v22 ... v2n >
					<   ...   ...   ...   ...   >
					< vk1 vk2 ... vkn >

	One Codeword <U> 	= 	<m> <G>
	[ 1 x n ]	       	=	< m1 m2 ... mk > < v11 v12 ... v1n >
                                                     < v21 v22 ... v2n >
                                                     < ... ... ... ... >
                                                     < vk1 vk2 ... vkn >

	* There are (2^k) messages <m> => (2^k) codewords <U>.  
	
	Generator <G> 		=	[ P | Ik ]	(Systematic Form) 
	[ k x n ] 
	
	Parity Matrix <P> 	=	< p11 p12 ... p1(n-k) >
	[ k x (n-k) ]	             	< p21 p22 ... p2(n-k) >
					< ... ... ... ...     >
					< pk1 pk2 ... pk(n-k) >

	Identity <Ik> 		= 	< 1  0 ...  0 >
	[ k x k ]	              	< 0  1 ...  0 >
					< ... ... ... ... >
					< 0  0 ...  1 >

	Parity Check <H> 	=	[ I(n-k) | P' ]
	[ (n-k) x n ] 

	* Matrix P' [ (n-k) x k ] is TRANSPOSE of the parity matrix P.
	* Matrix H' [ n x (n-k) ] is TRANSPOSE of the parity check matrix H.

	Received Vector <r> 	= 	< r1 r2 ... rn >
	[ 1 x n ]

	Syndrome of r, <S>	=	rH'
	[ 1 x (n-k) ]

	One Error Pattern <e> = 	< e1 e2 ... en >
	[ 1 x n ]

	Syndrome of e, <S>	=	eH'
	[ 1 x (n-k) ]

	* There are [ (2^n) - 1 ] different ERROR Patterns <e> including, 
			---- n^C_1 or 'n' -> 1-bit error patterns.
			---- n^C_2  2-bit error patterns.
			---- n^C_3  3-bit error patterns.
			---- n^C_4  4-bit error patterns.
			............................................................................
			---- n^C_n or  1 -> n-bit error pattern.

	*Standard Array is a [ 2^(n-k) x (2^k) ] marix with [2^(n-k) - 1] error patterns only.

The rate of the code is Rc = k/n information bits per codeword symbol <U>.  If the codeword symbols <U> are transmitted across the channel at a rate of Rs symbols/second, then the information rate associated with an (n, k) block code is Rb = Rc * Rs = kn Rs bits/second. Thus we see that block coding reduces the data rate compared to what we obtain with uncoded modulation by the code rate Rc.

Group codes are a special kind of block codes. They comprise a set of codewords, U1 U2 … UN, which contain the "all zeros" codeword (e.g. 00000) and exhibit a special property called closure. This property means that if any two valid codewords are subject to a bit wise EX - OR operation then they will produce another valid codeword in the set. 
