export function calcularMedia(numeros) {
  let soma = 0;
  numeros.forEach(x => soma += x);
  return soma / numeros.length;
}

export function calcularMediana(numeros) {
  const ordenado = [...numeros].sort((a, b) => a - b); //ordem crescente
  const meio = Math.floor(ordenado.length / 2);

  // Se o número de elementos for ímpar, retorna o do meio
  // Se for par, retorna a média dos dois do meio
  if (ordenado.length % 2 === 0) {
    return (ordenado[meio - 1] + ordenado[meio]) / 2;
  } else {
    return ordenado[meio];
  }
}

export function calcularModa(numeros) {
  const frequencia = {};
  let maxFreq = 0;

  // Conta a frequência de cada número
  for (const num of numeros) {
    frequencia[num] = (frequencia[num] || 0) + 1;
    if (frequencia[num] > maxFreq) {
      maxFreq = frequencia[num];
    }
  }

  // Pega todos os números que têm a frequência máxima
  const modas = Object.keys(frequencia)
    .filter(num => frequencia[num] === maxFreq);

  return modas.join('/');
}