// DigitalRune Engine - Copyright (C) DigitalRune GmbH
// This file is subject to the terms and conditions defined in
// file 'LICENSE.TXT', which is part of this source code package.

using System;


namespace DigitalRune.Mathematics.Algebra
{
  /// <summary>
  /// Computes the LU Decomposition of a matrix (single-precision).
  /// </summary>
  /// <remarks>
  /// <para>The LU Decomposition computes a unit lower triangular matrix L and an upper triangular
  /// matrix U for a matrix A so that <c>A' = L * U</c> where A' is a row-permutation of A.
  /// </para>
  /// <para> 
  /// The LU Decomposition with pivoting always exists, even if the matrix is singular. 
  /// </para>
  /// <para>
  /// Application: LU Decomposition is the preferred way to solve a linear set of equations. 
  /// This will fail if the matrix A is singular. 
  /// </para>
  /// <para>
  /// Use QR Decomposition for rectangular matrices A with m ≥ n.
  /// </para>
  /// </remarks>
  // JAMA documentation: If A is m x n with m >= n, then L is m x n and U is n x n. 
  // If m < n, then L is m x m and U is m x n.
  public class LUDecompositionF
  {
    //--------------------------------------------------------------
    #region Fields
    //--------------------------------------------------------------

    private readonly MatrixF _lu;
    private MatrixF _matrixL;
    private MatrixF _matrixU;
    private readonly int _m;
    private readonly int _n;
    private readonly int _pivotSign;          // +1 or -1
    private readonly int[] _pivotVector;
    #endregion


    //--------------------------------------------------------------
    #region Properties
    //--------------------------------------------------------------

    /// <summary>
    /// Gets the determinant of matrix A.
    /// </summary>
    /// <value>The determinant of matrix A.</value>
    /// <exception cref="MathematicsException">
    /// Matrix A is not square.
    /// </exception>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1065:DoNotRaiseExceptionsInUnexpectedLocations")]
    public float Determinant
    {
      get
      {
        if (_m != _n)
          throw new MathematicsException("Matrix A is not square.");

        if (!_determinant.HasValue)
        {
          float determinant = _pivotSign;
          for (int j = 0; j < _n; j++)
            determinant *= _lu[j, j];

          _determinant = determinant;
        }

        return _determinant.Value;
      }
    }
    private float? _determinant;


    /// <summary>
    /// Gets a value indicating whether the matrix U is numerically singular.
    /// </summary>
    /// <value>
    /// <see langword="true"/> if this the matrix U is numerically singular; otherwise, 
    /// <see langword="false"/>.
    /// </value>
    /// <remarks>
    /// <para>If U is singular A is singular too.</para>
    /// </remarks>
    public bool IsNumericallySingular
    {
      get
      {
        int n = Math.Min(_n, _m);
        for (int j = 0; j < n; j++)
          if (Numeric.IsZero(_lu[j, j]))
            return true;

        return false;
      }
    }


    /// <summary>
    /// Gets the lower triangular matrix L. (This property returns the internal matrix, 
    /// not a copy.)
    /// </summary>
    /// <value>The lower triangular matrix L.</value>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly")]
    public MatrixF L
    {
      get
      {
        if (_matrixL != null)
          return _matrixL;

        _matrixL = new MatrixF(_m, _n);
        for (int i = 0; i < _m; i++)
        {
          for (int j = 0; j < _n; j++)
          {
            if (i > j)
              _matrixL[i, j] = _lu[i, j];
            else if (i == j)
              _matrixL[i, j] = 1;
            //else
            //  _matrixL[i, j] = 0.0;
          }
        }
        return _matrixL;
      }
    }


    /// <summary>
    /// Gets the upper triangular matrix U. (This property returns the internal matrix, 
    /// not a copy.)
    /// </summary>
    /// <value>The upper triangular matrix U.</value>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly")]
    public MatrixF U
    {
      get
      {
        if (_matrixU != null)
          return _matrixU;

        _matrixU = new MatrixF(_n, _n);
        for (int i = 0; i < _n; i++)
        {
          for (int j = 0; j < _n; j++)
          {
            if (i <= j)
              _matrixU[i, j] = _lu[i, j];
            else
              _matrixU[i, j] = 0;
          }
        }
        return _matrixU;
      }
    }


    /// <summary>
    /// Gets the pivot permutation vector. (This property returns the internal array, 
    /// not a copy.)
    /// </summary>
    /// <value>The pivot permutation vector.</value>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays")]
    public int[] PivotPermutationVector
    {
      get { return _pivotVector; }
    }
    #endregion


    //--------------------------------------------------------------
    #region Creation and Cleanup
    //--------------------------------------------------------------

    /// <summary>
    /// Creates the LU decomposition of the given matrix.
    /// </summary>
    /// <param name="matrixA">
    /// The matrix A. (Can be rectangular. Number of rows ≥ number of columns.)
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="matrixA"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// The number of rows must be greater than or equal to the number of columns.
    /// </exception>
    public LUDecompositionF(MatrixF matrixA)
    {
      if (matrixA == null)
        throw new ArgumentNullException("matrixA");
      if (matrixA.NumberOfColumns > matrixA.NumberOfRows)
        throw new ArgumentException("The number of rows must be greater than or equal to the number of columns.", "matrixA");

      // Use a "left-looking", dot-product, Crout/Doolittle algorithm.
      _lu = matrixA.Clone();
      _m = matrixA.NumberOfRows;
      _n = matrixA.NumberOfColumns;
      _pivotVector = new int[_m];

      // Initialize with the 0 to m-1.
      for (int i = 0; i < _m; i++)
        _pivotVector[i] = i;

      _pivotSign = 1;

      // Outer loop.
      for (int j = 0; j < _n; j++)
      {
        // Make a copy of the j-th column to localize references.
        float[] luColumnJ = new float[_m];
        for (int i = 0; i < _m; i++)
          luColumnJ[i] = _lu[i, j];

        // Apply previous transformations.
        for (int i = 0; i < _m; i++)
        {
          // Most of the time is spent in the following dot product.
          int kmax = Math.Min(i, j);
          float s = 0;
          for (int k = 0; k < kmax; k++)
            s += _lu[i, k] * luColumnJ[k];

          luColumnJ[i] -= s;
          _lu[i, j] = luColumnJ[i];
        }

        // Find pivot and exchange if necessary.
        int p = j;
        for (int i = j + 1; i < _m; i++)
          if (Math.Abs(luColumnJ[i]) > Math.Abs(luColumnJ[p]))
            p = i;

        // Swap lines p and k. 
        if (p != j)
        {
          for (int k = 0; k < _n; k++)
          {
            float dummy = _lu[p, k];
            _lu[p, k] = _lu[j, k];
            _lu[j, k] = dummy;
          }

          MathHelper.Swap(ref _pivotVector[p], ref _pivotVector[j]);

          _pivotSign = -_pivotSign;
        }

        // Compute multipliers.
        if (j < _m && _lu[j, j] != 0)
          for (int i = j + 1; i < _m; i++)
            _lu[i, j] /= _lu[j, j];
      }
    }
    #endregion


    //--------------------------------------------------------------
    #region Methods
    //--------------------------------------------------------------

    /// <summary>
    /// Solves the equation <c>A * X = B</c>.
    /// </summary>
    /// <param name="matrixB">The matrix B with as many rows as A and any number of columns.</param>
    /// <returns>X, so that <c>A * X = B</c>.</returns>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="matrixB"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// The number of rows does not match.
    /// </exception>
    /// <exception cref="MathematicsException">
    /// The matrix A is numerically singular.
    /// </exception>
    public MatrixF SolveLinearEquations(MatrixF matrixB)
    {
      if (matrixB == null)
        throw new ArgumentNullException("matrixB");
      if (matrixB.NumberOfRows != _m)
        throw new ArgumentException("The number of rows does not match.", "matrixB");
      if (IsNumericallySingular)
        throw new MathematicsException("The original matrix A is numerically singular.");

      // Copy right hand side with pivoting
      MatrixF x = matrixB.GetSubmatrix(_pivotVector, 0, matrixB.NumberOfColumns-1);

      // Solve L*Y = B(piv,:)
      for (int k = 0; k < _n; k++)
        for (int i = k + 1; i < _n; i++)
          for (int j = 0; j < matrixB.NumberOfColumns; j++)
            x[i, j] -= x[k, j] * _lu[i, k];
      
      // Solve U*X = Y;
      for (int k = _n - 1; k >= 0; k--)
      {
        for (int j = 0; j < matrixB.NumberOfColumns; j++)
          x[k, j] /= _lu[k, k];
        for (int i = 0; i < k; i++)
          for (int j = 0; j < matrixB.NumberOfColumns; j++)
            x[i, j] -= x[k, j] * _lu[i, k];
      }
      return x;
    }
    #endregion
  }
}
