using System;
using CoreLocation;
using Foundation;
using MapKit;
using ObjCRuntime;

namespace AnnotationClustering
{
	// @interface FBQuadTreeNode : NSObject
	[BaseType (typeof(NSObject))]
	interface FBQuadTreeNode
	{
		// @property (assign, nonatomic) NSUInteger count;
		[Export ("count", ArgumentSemantic.Assign)]
		nuint Count { get; set; }

		// @property (assign, nonatomic) FBBoundingBox boundingBox;
		[Export ("boundingBox", ArgumentSemantic.Assign)]
		FBBoundingBox BoundingBox { get; set; }

		// @property (nonatomic, strong) NSMutableArray * annotations;
		[Export ("annotations", ArgumentSemantic.Strong)]
		NSMutableArray Annotations { get; set; }

		// @property (nonatomic, strong) FBQuadTreeNode * northEast;
		[Export ("northEast", ArgumentSemantic.Strong)]
		FBQuadTreeNode NorthEast { get; set; }

		// @property (nonatomic, strong) FBQuadTreeNode * northWest;
		[Export ("northWest", ArgumentSemantic.Strong)]
		FBQuadTreeNode NorthWest { get; set; }

		// @property (nonatomic, strong) FBQuadTreeNode * southEast;
		[Export ("southEast", ArgumentSemantic.Strong)]
		FBQuadTreeNode SouthEast { get; set; }

		// @property (nonatomic, strong) FBQuadTreeNode * southWest;
		[Export ("southWest", ArgumentSemantic.Strong)]
		FBQuadTreeNode SouthWest { get; set; }

		// -(id)initWithBoundingBox:(FBBoundingBox)box;
		[Export ("initWithBoundingBox:")]
		IntPtr Constructor (FBBoundingBox box);

		// -(BOOL)isLeaf;
		[Export ("isLeaf")]
		//[[Verify (MethodToProperty)]
		bool IsLeaf { get; }

		// -(void)subdivide;
		[Export ("subdivide")]
		void Subdivide ();
	}

	// @interface FBAnnotationCluster : NSObject <MKAnnotation>
	[BaseType (typeof(NSObject))]
	interface FBAnnotationCluster : IMKAnnotation
	{
		// @property (assign, nonatomic) CLLocationCoordinate2D coordinate;
		[Export ("coordinate", ArgumentSemantic.Assign)]
		CLLocationCoordinate2D Coordinate { get; set; }

		// @property (copy, nonatomic) NSString * title;
		[Export ("title")]
		string Title { get; set; }

		// @property (copy, nonatomic) NSString * subtitle;
		[Export ("subtitle")]
		string Subtitle { get; set; }

		// @property (nonatomic, strong) NSArray * annotations;
		[Export ("annotations", ArgumentSemantic.Strong)]
		//[[Verify (StronglyTypedNSArray)]
		NSObject[] Annotations { get; set; }
	}

	// @protocol FBClusteringManagerDelegate <NSObject>
	[Protocol, Model]
	[BaseType (typeof(NSObject))]
	interface FBClusteringManagerDelegate
	{
		// @optional -(CGFloat)cellSizeFactorForCoordinator:(FBClusteringManager *)coordinator;
		[Export ("cellSizeFactorForCoordinator:")]
		nfloat CellSizeFactorForCoordinator (FBClusteringManager coordinator);
	}

	// @interface FBClusteringManager : NSObject
	[BaseType (typeof(NSObject))]
	interface FBClusteringManager
	{
		[Wrap ("WeakDelegate")]
		FBClusteringManagerDelegate Delegate { get; set; }

		// @property (assign, nonatomic) id<FBClusteringManagerDelegate> delegate;
		[Export ("delegate", ArgumentSemantic.Assign)][NullAllowed]
		NSObject WeakDelegate { get; set; }

		// -(id)initWithAnnotations:(NSArray *)annotations;
		[Export ("initWithAnnotations:")]
		//[[Verify (StronglyTypedNSArray)]
		IntPtr Constructor (NSObject[] annotations);

		// -(void)setAnnotations:(NSArray *)annotations;
		[Export ("setAnnotations:")]
		//[[Verify (StronglyTypedNSArray)]
		void SetAnnotations (NSObject[] annotations);

		// -(void)addAnnotations:(NSArray *)annotations;
		[Export ("addAnnotations:")]
		//[Verify (StronglyTypedNSArray)]
		void AddAnnotations (NSObject[] annotations);

		// -(NSArray *)clusteredAnnotationsWithinMapRect:(MKMapRect)rect withZoomScale:(double)zoomScale;
		[Export ("clusteredAnnotationsWithinMapRect:withZoomScale:")]
		//[Verify (StronglyTypedNSArray)]
		NSObject[] ClusteredAnnotationsWithinMapRect (MKMapRect rect, double zoomScale);

		// -(NSArray *)allAnnotations;
		[Export ("allAnnotations")]
		//[Verify (MethodToProperty), Verify (StronglyTypedNSArray)]
		NSObject[] AllAnnotations { get; }

		// -(void)displayAnnotations:(NSArray *)annotations onMapView:(MKMapView *)mapView;
		[Export ("displayAnnotations:onMapView:")]
		//[Verify (StronglyTypedNSArray)]
		void DisplayAnnotations (NSObject[] annotations, MKMapView mapView);
	}
}
