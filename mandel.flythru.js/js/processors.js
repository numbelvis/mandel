/*

    Processing modes for the server.


    0 - CPU w/ Regular Math
    
    1 - CPU w/ Arbitrary Math

    2 - GPUs w/ Regular Math

    3 - GPUs w/ Arbitrary Math

*/

export { modes as default };

export let modes = new Map([
    
    [0, 'cpu-reg'],
    [1, 'cpu-arb'],
    [2, 'gpu-reg'],
    [3, 'gpu-arb'],
])